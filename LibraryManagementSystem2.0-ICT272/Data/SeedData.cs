using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
            IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider
                .GetRequiredService<ApplicationDbContext>();

            // ── Roles ─────────────────────────────────
            string[] roles = { "Admin", "Member" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
            }

            // ── Admin User ────────────────────────────
            string adminEmail = "admin@library.com";
            if (await userManager
                .FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Library",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    IsActive = true,
                    DateJoined = DateTime.Now
                };
                var result = await userManager
                    .CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                    await userManager
                        .AddToRoleAsync(admin, "Admin");
            }

            // ── Member User ───────────────────────────
            string memberEmail = "member@library.com";
            if (await userManager
                .FindByEmailAsync(memberEmail) == null)
            {
                var member = new ApplicationUser
                {
                    UserName = memberEmail,
                    Email = memberEmail,
                    FirstName = "Test",
                    LastName = "Member",
                    EmailConfirmed = true,
                    IsActive = true,
                    DateJoined = DateTime.Now
                };
                var result = await userManager
                    .CreateAsync(member, "Member@123");
                if (result.Succeeded)
                    await userManager
                        .AddToRoleAsync(member, "Member");
            }

            // ── Borrowing Rule ────────────────────────
            if (!await context.BorrowingRules.AnyAsync())
            {
                context.BorrowingRules.Add(new BorrowingRule
                {
                    RuleName = "Standard Member Rule",
                    LoanDurationDays = 14,
                    MaxRenewals = 2,
                    OverduePenaltyPerDay = 0.50m,
                    MaxBorrowableItems = 5,
                    IsActive = true,
                    Notes = "Default rule for all members"
                });
                await context.SaveChangesAsync();
            }

            // ── Library Profile ───────────────────────
            if (!await context.LibraryProfiles.AnyAsync())
            {
                context.LibraryProfiles.Add(new LibraryProfile
                {
                    LibraryName = "City Central Library",
                    Location = "123 Main Street, Sydney NSW 2000",
                    OperatingHours =
                        "Mon-Fri 9am-6pm, Sat 10am-4pm",
                    ContactNumber = "+61 2 1234 5678",
                    ContactEmail = "info@citycentral.com",
                    Description =
                        "Your community library providing " +
                        "access to thousands of books.",
                    EstablishedYear = 2005
                });
                await context.SaveChangesAsync();
            }
        }
    }
}