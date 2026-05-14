using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.TotalMembers = await _context.Users.CountAsync();
            ViewBag.ActiveBorrows = await _context.BorrowRecords
                .CountAsync(b =>
                    b.Status == BorrowStatus.Borrowed
                    || b.Status == BorrowStatus.Renewed);
            ViewBag.OverdueCount = await _context.BorrowRecords
                .CountAsync(b => b.Status == BorrowStatus.Overdue);

            return View();
        }

        // GET: /Admin/Members
        public async Task<IActionResult> Members()
        {
            var memberRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Member");

            List<ApplicationUser> members = new();

            if (memberRole != null)
            {
                var memberIds = await _context.UserRoles
                    .Where(ur => ur.RoleId == memberRole.Id)
                    .Select(ur => ur.UserId)
                    .ToListAsync();

                members = await _context.Users
                    .Where(u => memberIds.Contains(u.Id))
                    .OrderBy(u => u.FirstName)
                    .ToListAsync();
            }

            // Get borrow counts for each member
            var borrowCounts = await _context.BorrowRecords
                .GroupBy(b => b.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    Total = g.Count(),
                    Active = g.Count(b =>
                        b.Status == BorrowStatus.Borrowed
                        || b.Status == BorrowStatus.Renewed),
                    Overdue = g.Count(b =>
                        b.Status == BorrowStatus.Overdue)
                })
                .ToListAsync();

            ViewBag.BorrowCounts = borrowCounts;
            return View(members);
        }

        // GET: /Admin/MemberDetails/id
        public async Task<IActionResult> MemberDetails(string id)
        {
            var member = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (member == null)
                return NotFound();

            var borrows = await _context.BorrowRecords
                .Include(b => b.Book)
                .Include(b => b.Fine)
                .Where(b => b.UserId == id)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();

            var fines = await _context.Fines
                .Include(f => f.BorrowRecord)
                    .ThenInclude(b => b.Book)
                .Where(f => f.BorrowRecord.UserId == id)
                .ToListAsync();

            ViewBag.Borrows = borrows;
            ViewBag.Fines = fines;
            ViewBag.TotalFines = fines.Sum(f => f.Amount);
            ViewBag.UnpaidFines = fines
                .Where(f => !f.IsPaid).Sum(f => f.Amount);

            return View(member);
        }

        // POST: /Admin/ToggleMemberStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleMemberStatus(string id)
        {
            var member = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (member == null)
                return NotFound();

            member.IsActive = !member.IsActive;
            await _context.SaveChangesAsync();

            TempData["Success"] = member.IsActive
                ? $"{member.FullName} account activated."
                : $"{member.FullName} account deactivated.";

            return RedirectToAction(nameof(Members));
        }
    }
}