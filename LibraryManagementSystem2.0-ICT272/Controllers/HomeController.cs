using System.Diagnostics;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // ── Home Page ─────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            // We'll populate these with real data in later phases
            // For now just return the view
            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.TotalMembers = await _context.Users.CountAsync();
            ViewBag.ActiveBorrows = await _context.BorrowRecords
                .Where(b => b.Status == BorrowStatus.Borrowed)
                .CountAsync();

            return View();
        }

        // ── Privacy Page ──────────────────────────────────────
        public IActionResult Privacy()
        {
            return View();
        }

        // ── Error Page ────────────────────────────────────────
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}