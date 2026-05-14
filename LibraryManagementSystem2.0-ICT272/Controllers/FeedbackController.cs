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

        // GET: /Feedback/Create?bookId=5 — Member only
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);

            if (book == null)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);

            // Check if member already submitted feedback
            bool alreadyReviewed = await _context.Feedbacks
                .AnyAsync(f => f.UserId == user!.Id
                    && f.BookId == bookId);

            if (alreadyReviewed)
            {
                TempData["Error"] =
                    "You have already submitted a review for this book.";
                return RedirectToAction("Details", "Books",
                    new { id = bookId });
            }

            ViewBag.Book = book;
            return View(new Feedback { BookId = bookId });
        }

        // POST: /Feedback/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create(Feedback model)
        {
            var user = await _userManager.GetUserAsync(User);

            // Remove navigation properties from validation
            ModelState.Remove("User");
            ModelState.Remove("Book");

            if (!ModelState.IsValid)
            {
                ViewBag.Book = await _context.Books
                    .FindAsync(model.BookId);
                return View(model);
            }

            model.UserId = user!.Id;
            model.SubmittedDate = DateTime.Now;
            model.IsApproved = false;

            _context.Feedbacks.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Your review has been submitted and is " +
                "awaiting approval. Thank you!";
            return RedirectToAction("Details", "Books",
                new { id = model.BookId });
        }

        // GET: /Feedback/Manage — Admin only
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

        // POST: /Feedback/Approve/5 — Admin only
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);

            if (feedback == null)
                return NotFound();

            feedback.IsApproved = true;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review approved successfully!";
            return RedirectToAction(nameof(Manage));
        }

        // POST: /Feedback/Delete/5 — Admin only
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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