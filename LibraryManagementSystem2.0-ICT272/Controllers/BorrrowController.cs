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

        // ── ADMIN: View all borrow transactions ───────────────
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? status)
        {
            var query = _context.BorrowRecords
                .Include(b => b.User)
                .Include(b => b.Book)
                .Include(b => b.Fine)
                .AsQueryable();

            // Auto-mark overdue records
            var overdueRecords = await query
                .Where(b => b.Status == BorrowStatus.Borrowed
                    && b.DueDate < DateTime.Now)
                .ToListAsync();

            foreach (var record in overdueRecords)
                record.Status = BorrowStatus.Overdue;

            if (overdueRecords.Any())
                await _context.SaveChangesAsync();

            // Filter by status
            if (!string.IsNullOrEmpty(status) &&
                Enum.TryParse<BorrowStatus>(status, out var s))
            {
                query = query.Where(b => b.Status == s);
            }

            var records = await query
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();

            ViewBag.StatusFilter = status;
            return View(records);
        }

        // ── MEMBER: Borrow a book ─────────────────────────────
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Borrow(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);

            if (book == null)
                return NotFound();

            if (book.AvailableCopies <= 0)
            {
                TempData["Error"] =
                    "This book is not available for borrowing.";
                return RedirectToAction("Details", "Books",
                    new { id = bookId });
            }

            var user = await _userManager.GetUserAsync(User);

            // Get active borrowing rule
            var rule = await _context.BorrowingRules
                .Where(r => r.IsActive)
                .FirstOrDefaultAsync()
                ?? new BorrowingRule
                {
                    LoanDurationDays = 14,
                    MaxRenewals = 2,
                    MaxBorrowableItems = 5
                };

            // Check if member has reached max borrow limit
            int activeBorrows = await _context.BorrowRecords
                .CountAsync(b => b.UserId == user!.Id
                    && (b.Status == BorrowStatus.Borrowed
                        || b.Status == BorrowStatus.Overdue));

            if (activeBorrows >= rule.MaxBorrowableItems)
            {
                TempData["Error"] =
                    $"You have reached the maximum limit of " +
                    $"{rule.MaxBorrowableItems} borrowed books.";
                return RedirectToAction("MyBorrows");
            }

            // Check if member already borrowed this book
            bool alreadyBorrowed = await _context.BorrowRecords
                .AnyAsync(b => b.UserId == user!.Id
                    && b.BookId == bookId
                    && (b.Status == BorrowStatus.Borrowed
                        || b.Status == BorrowStatus.Overdue));

            if (alreadyBorrowed)
            {
                TempData["Error"] =
                    "You have already borrowed this book.";
                return RedirectToAction("MyBorrows");
            }

            // Create borrow record
            var borrowRecord = new BorrowRecord
            {
                UserId = user!.Id,
                BookId = bookId,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(
                    rule.LoanDurationDays),
                Status = BorrowStatus.Borrowed,
                RenewalsUsed = 0
            };

            // Decrease available copies
            book.AvailableCopies--;

            // Fulfill any pending reservation
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r =>
                    r.UserId == user.Id
                    && r.BookId == bookId
                    && r.Status == ReservationStatus.Pending);

            if (reservation != null)
                reservation.Status = ReservationStatus.Fulfilled;

            _context.BorrowRecords.Add(borrowRecord);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"You have successfully borrowed '{book.Title}'. " +
                $"Due date: {borrowRecord.DueDate:MMMM dd, yyyy}";
            return RedirectToAction(nameof(MyBorrows));
        }

        // ── MEMBER: Reserve a book ────────────────────────────
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Reserve(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);

            if (book == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            // Check for existing reservation
            bool alreadyReserved = await _context.Reservations
                .AnyAsync(r => r.UserId == user!.Id
                    && r.BookId == bookId
                    && r.Status == ReservationStatus.Pending);

            if (alreadyReserved)
            {
                TempData["Error"] =
                    "You have already reserved this book.";
                return RedirectToAction("Details", "Books",
                    new { id = bookId });
            }

            var reservation = new Reservation
            {
                UserId = user!.Id,
                BookId = bookId,
                ReservationDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(7),
                Status = ReservationStatus.Pending
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"'{book.Title}' has been reserved. " +
                $"We'll notify you when it's available. " +
                $"Reservation expires: " +
                $"{reservation.ExpiryDate:MMMM dd, yyyy}";
            return RedirectToAction(nameof(MyBorrows));
        }

        // ── MEMBER: View my borrows ───────────────────────────
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyBorrows()
        {
            var user = await _userManager.GetUserAsync(User);

            // Auto-mark overdue
            var overdue = await _context.BorrowRecords
                .Where(b => b.UserId == user!.Id
                    && b.Status == BorrowStatus.Borrowed
                    && b.DueDate < DateTime.Now)
                .ToListAsync();

            foreach (var r in overdue)
                r.Status = BorrowStatus.Overdue;

            if (overdue.Any())
                await _context.SaveChangesAsync();

            var borrows = await _context.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.Fine)
                .Where(b => b.UserId == user!.Id
                    && b.Status != BorrowStatus.Returned)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();

            var reservations = await _context.Reservations
                .Include(r => r.Book)
                .Where(r => r.UserId == user!.Id
                    && r.Status == ReservationStatus.Pending)
                .ToListAsync();

            ViewBag.Reservations = reservations;

            // Get active rule for renewal info
            var rule = await _context.BorrowingRules
                .Where(r => r.IsActive)
                .FirstOrDefaultAsync()
                ?? new BorrowingRule
                {
                    MaxRenewals = 2,
                    LoanDurationDays = 14
                };

            ViewBag.MaxRenewals = rule.MaxRenewals;
            ViewBag.LoanDays = rule.LoanDurationDays;

            return View(borrows);
        }

        // ── MEMBER: Renew a borrowed book ─────────────────────
        [Authorize(Roles = "Member")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Renew(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var record = await _context.BorrowRecords
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == id
                    && b.UserId == user!.Id);

            if (record == null)
                return NotFound();

            var rule = await _context.BorrowingRules
                .Where(r => r.IsActive)
                .FirstOrDefaultAsync()
                ?? new BorrowingRule
                {
                    MaxRenewals = 2,
                    LoanDurationDays = 14
                };

            if (record.RenewalsUsed >= rule.MaxRenewals)
            {
                TempData["Error"] =
                    $"You have reached the maximum of " +
                    $"{rule.MaxRenewals} renewals for this book.";
                return RedirectToAction(nameof(MyBorrows));
            }

            if (record.Status == BorrowStatus.Overdue)
            {
                TempData["Error"] =
                    "Overdue books cannot be renewed. " +
                    "Please return the book and pay any fines first.";
                return RedirectToAction(nameof(MyBorrows));
            }

            // Extend due date
            record.DueDate = record.DueDate.AddDays(
                rule.LoanDurationDays);
            record.RenewalsUsed++;
            record.Status = BorrowStatus.Renewed;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"'{record.Book.Title}' renewed successfully! " +
                $"New due date: {record.DueDate:MMMM dd, yyyy}";
            return RedirectToAction(nameof(MyBorrows));
        }

        // ── ADMIN: Return a book ──────────────────────────────
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id)
        {
            var record = await _context.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.Fine)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (record == null)
                return NotFound();

            record.ReturnDate = DateTime.Now;
            record.Status = BorrowStatus.Returned;

            // Increase available copies
            record.Book.AvailableCopies++;

            // Calculate fine if overdue
            if (DateTime.Now > record.DueDate &&
                record.Fine == null)
            {
                var rule = await _context.BorrowingRules
                    .Where(r => r.IsActive)
                    .FirstOrDefaultAsync()
                    ?? new BorrowingRule
                    {
                        OverduePenaltyPerDay = 0.50m
                    };

                int daysOverdue = (int)(DateTime.Now
                    - record.DueDate).TotalDays;

                if (daysOverdue > 0)
                {
                    var fine = new Fine
                    {
                        BorrowRecordId = record.Id,
                        Amount = daysOverdue *
                            rule.OverduePenaltyPerDay,
                        IssuedDate = DateTime.Now,
                        IsPaid = false,
                        Reason = $"Overdue by {daysOverdue} day(s)"
                    };
                    _context.Fines.Add(fine);
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"'{record.Book.Title}' returned successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ── MEMBER: Borrow history ────────────────────────────
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);

            var history = await _context.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.Fine)
                .Where(b => b.UserId == user!.Id)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();

            return View(history);
        }

        // ── ADMIN: Cancel a reservation ───────────────────────
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var reservation = await _context.Reservations
                .FindAsync(id);

            if (reservation == null)
                return NotFound();

            reservation.Status = ReservationStatus.Cancelled;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Reservation cancelled.";
            return RedirectToAction(nameof(Index));
        }
    }
}