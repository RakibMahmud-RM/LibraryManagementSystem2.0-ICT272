using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class LibraryProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LibraryProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /LibraryProfile — Public view
        public async Task<IActionResult> Index()
        {
            var profile = await _context.LibraryProfiles
                .FirstOrDefaultAsync();
            return View(profile);
        }

        // GET: /LibraryProfile/Edit — Admin only
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit()
        {
            var profile = await _context.LibraryProfiles
                .FirstOrDefaultAsync();

            // If no profile exists yet, create a blank one
            if (profile == null)
                profile = new LibraryProfile();

            return View(profile);
        }

        // POST: /LibraryProfile/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(LibraryProfile model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existing = await _context.LibraryProfiles
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                // Create new profile
                _context.LibraryProfiles.Add(model);
            }
            else
            {
                // Update existing profile
                existing.LibraryName = model.LibraryName;
                existing.Location = model.Location;
                existing.OperatingHours = model.OperatingHours;
                existing.ContactNumber = model.ContactNumber;
                existing.ContactEmail = model.ContactEmail;
                existing.Description = model.Description;
                existing.EstablishedYear = model.EstablishedYear;
            }

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Library profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}