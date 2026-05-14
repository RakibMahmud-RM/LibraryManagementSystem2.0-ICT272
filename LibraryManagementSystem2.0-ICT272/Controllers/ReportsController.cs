using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Total counts
            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.TotalMembers = await _context.Users
                .CountAsync();
            ViewBag.TotalBorrows = await _context.BorrowRecords
                .CountAsync();
            ViewBag.TotalFines = await _context.Fines
                .SumAsync(f => f.Amount);

            // Active borrows
            ViewBag.ActiveBorrows = await _context.BorrowRecords
                .CountAsync(b =>
                    b.Status == BorrowStatus.Borrowed
                    || b.Status == BorrowStatus.Renewed);

            // Overdue books
            ViewBag.OverdueCount = await _context.BorrowRecords
                .CountAsync(b => b.Status == BorrowStatus.Overdue);

            // Unpaid fines
            ViewBag.UnpaidFines = await _context.Fines
                .Where(f => !f.IsPaid)
                .SumAsync(f => f.Amount);

            // Most popular books (top 5)
            var popularBooks = await _context.BorrowRecords
                .Include(b => b.Book)
                .GroupBy(b => new {
                    b.BookId,
                    b.Book.Title,
                    b.Book.Author
                })
                .Select(g => new
                {
                    g.Key.BookId,
                    g.Key.Title,
                    g.Key.Author,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .Take(5)
                .ToListAsync();

            ViewBag.PopularBooks = popularBooks;

            // Most active members (top 5)
            var activeMembers = await _context.BorrowRecords
                .Include(b => b.User)
                .GroupBy(b => new {
                    b.UserId,
                    b.User.FirstName,
                    b.User.LastName,
                    b.User.Email
                })
                .Select(g => new
                {
                    g.Key.UserId,
                    g.Key.FirstName,
                    g.Key.LastName,
                    g.Key.Email,
                    BorrowCount = g.Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .Take(5)
                .ToListAsync();

            ViewBag.ActiveMembers = activeMembers;

            // Overdue records
            var overdueRecords = await _context.BorrowRecords
                .Include(b => b.User)
                .Include(b => b.Book)
                .Where(b => b.Status == BorrowStatus.Overdue)
                .OrderBy(b => b.DueDate)
                .ToListAsync();

            ViewBag.OverdueRecords = overdueRecords;

            // Borrowing trend (last 7 days)
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-i))
                .Reverse()
                .ToList();

            var borrowTrend = new List<object>();
            foreach (var day in last7Days)
            {
                int count = await _context.BorrowRecords
                    .CountAsync(b => b.BorrowDate.Date == day.Date);
                borrowTrend.Add(new
                {
                    Date = day.ToString("MMM dd"),
                    Count = count
                });
            }
            ViewBag.BorrowTrend = borrowTrend;

            // Category popularity
            var categoryStats = await _context.Categories
                .Include(c => c.BookCategories)
                    .ThenInclude(bc => bc.Book)
                        .ThenInclude(b => b.BorrowRecords)
                .Select(c => new
                {
                    c.Name,
                    BookCount = c.BookCategories.Count,
                    BorrowCount = c.BookCategories
                        .SelectMany(bc => bc.Book.BorrowRecords)
                        .Count()
                })
                .OrderByDescending(x => x.BorrowCount)
                .ToListAsync();

            ViewBag.CategoryStats = categoryStats;

            return View();
        }
    }
}