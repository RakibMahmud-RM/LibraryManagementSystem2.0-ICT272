using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    public class FinesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FinesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Admin: View all fines
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var fines = await _context.Fines
                .Include(f => f.BorrowRecord)
                    .ThenInclude(b => b.User)
                .Include(f => f.BorrowRecord)
                    .ThenInclude(b => b.Book)
                .OrderByDescending(f => f.IssuedDate)
                .ToListAsync();

            return View(fines);
        }

        // Admin: Mark fine as paid
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var fine = await _context.Fines.FindAsync(id);

            if (fine == null)
                return NotFound();

            fine.IsPaid = true;
            fine.PaidDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Fine marked as paid!";
            return RedirectToAction(nameof(Index));
        }

        // Member: View my fines
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyFines()
        {
            var user = await _userManager.GetUserAsync(User);

            var fines = await _context.Fines
                .Include(f => f.BorrowRecord)
                    .ThenInclude(b => b.Book)
                .Where(f => f.BorrowRecord.UserId == user!.Id)
                .OrderByDescending(f => f.IssuedDate)
                .ToListAsync();

            return View(fines);
        }
    }
}