using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BooksController(
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Books/Index — Browse all books (Public)
        public async Task<IActionResult> Index(string? search,
            int? categoryId)
        {
            var query = _context.Books
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .Where(b => b.IsActive)
                .AsQueryable();

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b =>
                    b.Title.Contains(search) ||
                    b.Author.Contains(search) ||
                    b.ISBN.Contains(search));
            }

            // Category filter
            if (categoryId.HasValue)
            {
                query = query.Where(b =>
                    b.BookCategories.Any(bc =>
                        bc.CategoryId == categoryId));
            }

            var books = await query
                .OrderByDescending(b => b.DateAdded)
                .ToListAsync();

            // For category filter dropdown
            ViewBag.Categories = await _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();

            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;

            return View(books);
        }

        // GET: /Books/Details/5 — View book details (Public)
        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .Include(b => b.Feedbacks)
                    .ThenInclude(f => f.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        // GET: /Books/Create — Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new BookViewModel
            {
                AvailableCategories = await GetCategoryListAsync()
            };
            return View(viewModel);
        }

        // POST: /Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(BookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableCategories = await GetCategoryListAsync();
                return View(model);
            }

            var book = new Book
            {
                Title = model.Title,
                Author = model.Author,
                ISBN = model.ISBN,
                Description = model.Description,
                PublishedYear = model.PublishedYear,
                Publisher = model.Publisher,
                TotalCopies = model.TotalCopies,
                AvailableCopies = model.TotalCopies,
                IsActive = model.IsActive,
                DateAdded = DateTime.Now
            };

            // Handle image upload
            if (model.CoverImage != null &&
                model.CoverImage.Length > 0)
            {
                book.CoverImagePath = await SaveImageAsync(
                    model.CoverImage);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Save categories
            if (model.SelectedCategoryIds.Any())
            {
                foreach (var catId in model.SelectedCategoryIds)
                {
                    _context.BookCategories.Add(new BookCategory
                    {
                        BookId = book.Id,
                        CategoryId = catId
                    });
                }
                await _context.SaveChangesAsync();
            }

            TempData["Success"] =
                $"Book '{book.Title}' added successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Books/Edit/5 — Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books
                .Include(b => b.BookCategories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            var viewModel = new BookViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Description = book.Description,
                PublishedYear = book.PublishedYear,
                Publisher = book.Publisher,
                TotalCopies = book.TotalCopies,
                IsActive = book.IsActive,
                ExistingCoverImagePath = book.CoverImagePath,
                SelectedCategoryIds = book.BookCategories
                    .Select(bc => bc.CategoryId).ToList(),
                AvailableCategories = await GetCategoryListAsync()
            };

            return View(viewModel);
        }

        // POST: /Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id,
            BookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableCategories = await GetCategoryListAsync();
                return View(model);
            }

            var book = await _context.Books
                .Include(b => b.BookCategories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            // Update fields
            book.Title = model.Title;
            book.Author = model.Author;
            book.ISBN = model.ISBN;
            book.Description = model.Description;
            book.PublishedYear = model.PublishedYear;
            book.Publisher = model.Publisher;
            book.IsActive = model.IsActive;

            // Update copies — adjust available if total changed
            int diff = model.TotalCopies - book.TotalCopies;
            book.TotalCopies = model.TotalCopies;
            book.AvailableCopies = Math.Max(0,
                book.AvailableCopies + diff);

            // Handle new image upload
            if (model.CoverImage != null &&
                model.CoverImage.Length > 0)
            {
                // Delete old image
                DeleteImage(book.CoverImagePath);
                book.CoverImagePath = await SaveImageAsync(
                    model.CoverImage);
            }

            // Update categories
            _context.BookCategories.RemoveRange(book.BookCategories);
            if (model.SelectedCategoryIds.Any())
            {
                foreach (var catId in model.SelectedCategoryIds)
                {
                    _context.BookCategories.Add(new BookCategory
                    {
                        BookId = book.Id,
                        CategoryId = catId
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Book '{book.Title}' updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Books/Delete/5 — Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        // POST: /Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            // Delete cover image file
            DeleteImage(book.CoverImagePath);

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Book '{book.Title}' deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ── Private Helpers ───────────────────────────────────

        private async Task<List<SelectListItem>> GetCategoryListAsync()
        {
            return await _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();
        }

        private async Task<string> SaveImageAsync(IFormFile image)
        {
            string uploadsFolder = Path.Combine(
                _webHostEnvironment.WebRootPath, "uploads", "covers");

            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString()
                + "_" + image.FileName;

            string filePath = Path.Combine(
                uploadsFolder, uniqueFileName);

            using var fileStream = new FileStream(
                filePath, FileMode.Create);
            await image.CopyToAsync(fileStream);

            return "/uploads/covers/" + uniqueFileName;
        }

        private void DeleteImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            string fullPath = Path.Combine(
                _webHostEnvironment.WebRootPath,
                imagePath.TrimStart('/'));

            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}