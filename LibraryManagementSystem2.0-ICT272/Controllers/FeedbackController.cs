using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
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

        // GET: /Feedback/Create?bookId=5
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                return NotFound();

            ViewBag.Book = book;

            var feedback = new Feedback
            {
                BookId = bookId
            };

            return View(feedback);
        }

        // POST: /Feedback/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create(
            int bookId, int rating, string? comment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var book = await _context.Books.FindAsync(bookId);
            if (book == null)
                return NotFound();

            if (rating < 1 || rating > 5)
            {
                ViewBag.Book = book;
                ViewBag.Error = "Please select a rating.";
                return View(new Feedback { BookId = bookId });
            }

            // Check if already reviewed
            bool alreadyReviewed = await _context.Feedbacks
                .AnyAsync(f =>
                    f.UserId == user.Id &&
                    f.BookId == bookId);

            if (alreadyReviewed)
            {
                TempData["Error"] =
                    "You have already reviewed this book.";
                return RedirectToAction(
                    "Details", "Books",
                    new { id = bookId });
            }

            var feedback = new Feedback
            {
                UserId = user.Id,
                BookId = bookId,
                Rating = rating,
                Comment = comment,
                SubmittedDate = DateTime.Now,
                IsApproved = false
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Your review has been submitted " +
                "and is pending approval.";
            return RedirectToAction(
                "Details", "Books",
                new { id = bookId });
        }

        // GET: /Feedback/Manage — Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .Include(f => f.Book)
                .OrderByDescending(f => f.SubmittedDate)
                .ToListAsync();

            return View(feedbacks);
        }

        // POST: /Feedback/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
                return NotFound();

            feedback.IsApproved = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review approved.";
            return RedirectToAction(nameof(Manage));
        }

        // POST: /Feedback/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
                return NotFound();

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review deleted.";
            return RedirectToAction(nameof(Manage));
        }
    }
}