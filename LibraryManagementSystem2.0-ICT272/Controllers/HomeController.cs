using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalBooks = await _context.Books
                .CountAsync(b => b.IsActive);
            ViewBag.TotalMembers = await _context.Users
                .CountAsync();
            ViewBag.ActiveBorrows = await _context.BorrowRecords
                .CountAsync(b =>
                    b.Status == BorrowStatus.Borrowed ||
                    b.Status == BorrowStatus.Renewed);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}