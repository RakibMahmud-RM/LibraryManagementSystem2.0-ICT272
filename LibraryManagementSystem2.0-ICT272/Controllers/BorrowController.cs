using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class BorrowController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public BorrowController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? status)
        {
            var query = _context.BorrowRecords
                .Include(b => b.User)
                .Include(b => b.Book)
                .Include(b => b.Fine)
                .AsQueryable();

            var overdueRecords = await query
                .Where(b =>
                    b.Status == BorrowStatus.Borrowed &&
                    b.DueDate < DateTime.Now)
                .ToListAsync();

            foreach (var record in overdueRecords)
                record.Status = BorrowStatus.Overdue;

            if (overdueRecords.Any())
                await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(status) &&
                Enum.TryParse<BorrowStatus>(status, out var s))
                query = query.Where(b => b.Status == s);

            var records = await query
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();

            ViewBag.StatusFilter = status;
            return View(records);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return NotFound();

            if (book.AvailableCopies <= 0)
            {
                TempData["Error"] = "This book is not available.";
                return RedirectToAction(
                    "Details", "Books", new { id = bookId });
            }

            var rule = await _context.BorrowingRules
                .FirstOrDefaultAsync(r => r.IsActive);
            int maxItems = rule?.MaxBorrowableItems ?? 5;

            int currentBorrows = await _context.BorrowRecords
                .CountAsync(b =>
                    b.UserId == user.Id &&
                    (b.Status == BorrowStatus.Borrowed ||
                     b.Status == BorrowStatus.Renewed));

            if (currentBorrows >= maxItems)
            {
                TempData["Error"] =
                    $"Maximum limit of {maxItems} books reached.";
                return RedirectToAction(nameof(MyBorrows));
            }

            bool alreadyBorrowed = await _context.BorrowRecords
                .AnyAsync(b =>
                    b.UserId == user.Id &&
                    b.BookId == bookId &&
                    (b.Status == BorrowStatus.Borrowed ||
                     b.Status == BorrowStatus.Renewed));

            if (alreadyBorrowed)
            {
                TempData["Error"] =
                    "You already have this book borrowed.";
                return RedirectToAction(nameof(MyBorrows));
            }

            int loanDays = rule?.LoanDurationDays ?? 14;
            var borrowRecord = new BorrowRecord
            {
                UserId = user.Id,
                BookId = bookId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(loanDays),
                Status = BorrowStatus.Borrowed,
                RenewalsUsed = 0
            };

            book.AvailableCopies--;
            _context.BorrowRecords.Add(borrowRecord);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"You borrowed '{book.Title}'. " +
                $"Due: {borrowRecord.DueDate:MMM dd, yyyy}";
            return RedirectToAction(nameof(MyBorrows));
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyBorrows()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var overdue = await _context.BorrowRecords
                .Where(b =>
                    b.UserId == user.Id &&
                    b.DueDate < DateTime.Now &&
                    (b.Status == BorrowStatus.Borrowed ||
                     b.Status == BorrowStatus.Renewed))
                .ToListAsync();

            foreach (var r in overdue)
                r.Status = BorrowStatus.Overdue;

            if (overdue.Any())
                await _context.SaveChangesAsync();

            var borrows = await _context.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.Fine)
                .Where(b =>
                    b.UserId == user.Id &&
                    b.Status != BorrowStatus.Returned)
                .OrderBy(b => b.DueDate)
                .ToListAsync();

            return View(borrows);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Renew(int borrowId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var borrow = await _context.BorrowRecords
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b =>
                    b.Id == borrowId &&
                    b.UserId == user.Id);

            if (borrow == null) return NotFound();

            var rule = await _context.BorrowingRules
                .FirstOrDefaultAsync(r => r.IsActive);
            int maxRenewals = rule?.MaxRenewals ?? 2;

            if (borrow.RenewalsUsed >= maxRenewals)
            {
                TempData["Error"] =
                    $"Maximum renewals ({maxRenewals}) reached.";
                return RedirectToAction(nameof(MyBorrows));
            }

            borrow.DueDate = DateTime.Now
                .AddDays(rule?.LoanDurationDays ?? 14);
            borrow.RenewalsUsed++;
            borrow.Status = BorrowStatus.Renewed;
            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"'{borrow.Book.Title}' renewed. " +
                $"Due: {borrow.DueDate:MMM dd, yyyy}";
            return RedirectToAction(nameof(MyBorrows));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Return(int borrowId)
        {
            var borrow = await _context.BorrowRecords
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == borrowId);

            if (borrow == null) return NotFound();

            borrow.ReturnDate = DateTime.Now;
            borrow.Status = BorrowStatus.Returned;
            borrow.Book.AvailableCopies++;

            if (DateTime.Now > borrow.DueDate)
            {
                var rule = await _context.BorrowingRules
                    .FirstOrDefaultAsync(r => r.IsActive);
                decimal penalty =
                    rule?.OverduePenaltyPerDay ?? 0.50m;
                int days = (int)(DateTime.Now
                    - borrow.DueDate).TotalDays;

                _context.Fines.Add(new Fine
                {
                    BorrowRecordId = borrow.Id,
                    Amount = days * penalty,
                    Reason = $"Overdue by {days} day(s)",
                    IssuedDate = DateTime.Now,
                    IsPaid = false
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] =
                $"'{borrow.Book.Title}' returned successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var borrows = await _context.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.Fine)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();

            return View(borrows);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Reserve(int bookId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            bool alreadyReserved = await _context.Reservations
                .AnyAsync(r =>
                    r.UserId == user.Id &&
                    r.BookId == bookId &&
                    r.Status == ReservationStatus.Pending);

            if (alreadyReserved)
            {
                TempData["Error"] =
                    "You already reserved this book.";
                return RedirectToAction(
                    "Details", "Books", new { id = bookId });
            }

            _context.Reservations.Add(new Reservation
            {
                UserId = user.Id,
                BookId = bookId,
                ReservationDate = DateTime.Now,
                Status = ReservationStatus.Pending
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = "Book reserved successfully!";
            return RedirectToAction(nameof(MyBorrows));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> CancelReservation(
            int reservationId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r =>
                    r.Id == reservationId &&
                    r.UserId == user.Id);

            if (reservation == null) return NotFound();

            reservation.Status = ReservationStatus.Cancelled;
            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Reservation cancelled successfully.";
            return RedirectToAction(nameof(MyBorrows));
        }
    }
}