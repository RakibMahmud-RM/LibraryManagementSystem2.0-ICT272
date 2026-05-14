using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .Include(c => c.BookCategories)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(categories);
        }

        // GET: /Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Check for duplicate
            bool exists = await _context.Categories
                .AnyAsync(c => c.Name == model.Name);

            if (exists)
            {
                ModelState.AddModelError("Name",
                    "A category with this name already exists.");
                return View(model);
            }

            _context.Categories.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Category '{model.Name}' created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.Categories
                .FindAsync(id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: /Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var category = await _context.Categories.FindAsync(id);

            if (category == null)
                return NotFound();

            category.Name = model.Name;
            category.Description = model.Description;

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Category '{category.Name}' updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories
                .Include(c => c.BookCategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            // Check if category is used by any books
            if (category.BookCategories.Any())
            {
                TempData["Error"] =
                    $"Cannot delete '{category.Name}' " +
                    $"— it is assigned to " +
                    $"{category.BookCategories.Count} book(s). " +
                    $"Remove the category from those books first.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Category '{category.Name}' deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}