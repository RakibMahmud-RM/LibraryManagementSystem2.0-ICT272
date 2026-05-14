using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BorrowingRulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowingRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /BorrowingRules
        public async Task<IActionResult> Index()
        {
            var rules = await _context.BorrowingRules
                .OrderByDescending(r => r.IsActive)
                .ToListAsync();
            return View(rules);
        }

        // GET: /BorrowingRules/Create
        public IActionResult Create()
        {
            return View(new BorrowingRule());
        }

        // POST: /BorrowingRules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BorrowingRule model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _context.BorrowingRules.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Rule '{model.RuleName}' created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /BorrowingRules/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var rule = await _context.BorrowingRules.FindAsync(id);

            if (rule == null)
                return NotFound();

            return View(rule);
        }

        // POST: /BorrowingRules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            BorrowingRule model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var rule = await _context.BorrowingRules.FindAsync(id);

            if (rule == null)
                return NotFound();

            rule.RuleName = model.RuleName;
            rule.LoanDurationDays = model.LoanDurationDays;
            rule.MaxRenewals = model.MaxRenewals;
            rule.OverduePenaltyPerDay = model.OverduePenaltyPerDay;
            rule.MaxBorrowableItems = model.MaxBorrowableItems;
            rule.IsActive = model.IsActive;
            rule.Notes = model.Notes;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Rule '{rule.RuleName}' updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /BorrowingRules/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var rule = await _context.BorrowingRules.FindAsync(id);

            if (rule == null)
                return NotFound();

            _context.BorrowingRules.Remove(rule);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Borrowing rule deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}