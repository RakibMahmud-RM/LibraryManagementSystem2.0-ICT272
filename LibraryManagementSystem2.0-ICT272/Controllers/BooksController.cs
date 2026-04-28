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

        // GET: /Borrow/Index — Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var borrows = await _context.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.User)
                .Include(b => b.Fine)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();

            return View(borrows);
        }

        // POST: /Borrow/BorrowBook
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var user = await _userManager
                .GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
                return NotFound();

            if (book.AvailableCopies <= 0)
            {
                TempData["Error"] =
                    "This book is not available.";
                return RedirectToAction(
                    "Details", "Books",
                    new { id = bookId });
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
                    $"You have reached the maximum " +
                    $"borrowing limit of {maxItems} books.";
                return RedirectToAction(
                    "Details", "Books",
                    new { id = bookId });
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
                return RedirectToAction(
                    "Details", "Books",
                    new { id = bookId });
            }

            int loanDays = rule?.LoanDurationDays ?? 14;

            var borrowRecord = new BorrowRecord
            {
                UserId = user.Id,
                BookId = bookId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(loanDays),
                Status = BorrowStatus.Borrowed
            };

            book.AvailableCopies--;
            _context.BorrowRecords.Add(borrowRecord);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"You have borrowed '{book.Title}'. " +
                $"Due date: {borrowRecord.DueDate:MMM dd, yyyy}";

            return RedirectToAction(nameof(MyBorrows));
        }

        // GET: /Borrow/MyBorrows
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyBorrows()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Auto-mark overdue
            var overdueRecords = await _context.BorrowRecords
                .Where(b =>
                    b.UserId == user.Id &&
                    b.DueDate < DateTime.Now &&
                    (b.Status == BorrowStatus.Borrowed ||
                     b.Status == BorrowStatus.Renewed))
                .ToListAsync();

            foreach (var record in overdueRecords)
                record.Status = BorrowStatus.Overdue;

            if (overdueRecords.Any())
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

        // POST: /Borrow/Renew
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

            if (borrow == null)
                return NotFound();

            var rule = await _context.BorrowingRules
                .FirstOrDefaultAsync(r => r.IsActive);
            int maxRenewals = rule?.MaxRenewals ?? 2;

            if (borrow.RenewalCount >= maxRenewals)
            {
                TempData["Error"] =
                    $"Maximum renewals ({maxRenewals}) " +
                    $"reached for this book.";
                return RedirectToAction(nameof(MyBorrows));
            }

            int loanDays = rule?.LoanDurationDays ?? 14;
            borrow.DueDate = DateTime.Now.AddDays(loanDays);
            borrow.RenewalCount++;
            borrow.Status = BorrowStatus.Renewed;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"'{borrow.Book.Title}' renewed. " +
                $"New due date: {borrow.DueDate:MMM dd, yyyy}";

            return RedirectToAction(nameof(MyBorrows));
        }

        // POST: /Borrow/Return — Admin only
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Return(int borrowId)
        {
            var borrow = await _context.BorrowRecords
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == borrowId);

            if (borrow == null)
                return NotFound();

            borrow.ReturnDate = DateTime.Now;
            borrow.Status = BorrowStatus.Returned;
            borrow.Book.AvailableCopies++;

            // Calculate fine if overdue
            if (DateTime.Now > borrow.DueDate)
            {
                var rule = await _context.BorrowingRules
                    .FirstOrDefaultAsync(r => r.IsActive);
                decimal penaltyPerDay =
                    rule?.OverduePenaltyPerDay ?? 0.50m;

                int daysOverdue = (int)(DateTime.Now
                    - borrow.DueDate).TotalDays;
                decimal fineAmount =
                    daysOverdue * penaltyPerDay;

                var fine = new Fine
                {
                    BorrowRecordId = borrow.Id,
                    Amount = fineAmount,
                    Reason =
                        $"Overdue by {daysOverdue} day(s)",
                    IssuedDate = DateTime.Now,
                    IsPaid = false
                };

                _context.Fines.Add(fine);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"'{borrow.Book.Title}' returned successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Borrow/History
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

        // POST: /Borrow/Reserve
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
                    "You already have a reservation for this book.";
                return RedirectToAction(
                    "Details", "Books",
                    new { id = bookId });
            }

            var reservation = new Reservation
            {
                UserId = user.Id,
                BookId = bookId,
                ReservationDate = DateTime.Now,
                Status = ReservationStatus.Pending
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Book reserved successfully! " +
                "We will notify you when it is available.";

            return RedirectToAction(nameof(MyBorrows));
        }

        // POST: /Borrow/CancelReservation
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

            if (reservation == null)
                return NotFound();

            reservation.Status = ReservationStatus.Cancelled;
            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Reservation cancelled successfully.";

            return RedirectToAction(nameof(MyBorrows));
        }
    }
}