using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FeedbackController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);

            bool alreadyReviewed = await _context.Feedbacks
                .AnyAsync(f => f.UserId == user!.Id
                    && f.BookId == bookId);

            if (alreadyReviewed)
            {
                TempData["Error"] =
                    "You have already reviewed this book.";
                return RedirectToAction("Details", "Books",
                    new { id = bookId });
            }

            ViewBag.Book = book;
            return View(new Feedback
            {
                BookId = bookId,
                UserId = user!.Id
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create(
            int bookId, int rating, string? comment)
        {
            var user = await _userManager.GetUserAsync(User);
            var book = await _context.Books.FindAsync(bookId);

            if (book == null) return NotFound();

            if (rating < 1 || rating > 5)
            {
                ViewBag.Book = book;
                ViewBag.Error =
                    "Please select a rating between 1 and 5.";
                return View(new Feedback
                {
                    BookId = bookId,
                    UserId = user!.Id
                });
            }

            bool alreadyReviewed = await _context.Feedbacks
                .AnyAsync(f => f.UserId == user!.Id
                    && f.BookId == bookId);

            if (alreadyReviewed)
            {
                TempData["Error"] =
                    "You have already reviewed this book.";
                return RedirectToAction("Details", "Books",
                    new { id = bookId });
            }

            var feedback = new Feedback
            {
                UserId = user!.Id,
                BookId = bookId,
                Rating = rating,
                Comment = comment,
                SubmittedDate = DateTime.Now,
                IsApproved = false
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Review submitted! Awaiting approval.";
            return RedirectToAction("Details", "Books",
                new { id = bookId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage(string? filter)
        {
            var query = _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Book)
                .AsQueryable();

            if (filter == "pending")
                query = query.Where(f => !f.IsApproved);
            else if (filter == "approved")
                query = query.Where(f => f.IsApproved);

            var feedbacks = await query
                .OrderByDescending(f => f.SubmittedDate)
                .ToListAsync();

            ViewBag.Filter = filter;
            ViewBag.PendingCount = await _context.Feedbacks
                .CountAsync(f => !f.IsApproved);

            return View(feedbacks);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound();

            feedback.IsApproved = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review approved!";
            return RedirectToAction(nameof(Manage));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null) return NotFound();

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review deleted.";
            return RedirectToAction(nameof(Manage));
        }
    }
}