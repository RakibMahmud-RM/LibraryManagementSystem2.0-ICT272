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

            // ── Categories ────────────────────────────
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new() { Name = "Fiction",
                        Description = "Fictional stories" },
                    new() { Name = "Non-Fiction",
                        Description = "Real world facts" },
                    new() { Name = "Science",
                        Description = "Science topics" },
                    new() { Name = "Technology",
                        Description = "Tech and computing" },
                    new() { Name = "History",
                        Description = "Historical events" },
                    new() { Name = "Mystery",
                        Description = "Mystery novels" },
                    new() { Name = "Biography",
                        Description = "Life stories" },
                    new() { Name = "Self-Help",
                        Description = "Personal development" },
                    new() { Name = "Fantasy",
                        Description = "Fantasy worlds" },
                    new() { Name = "Science Fiction",
                        Description = "Sci-Fi stories" }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
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

            // ── Sample Books ──────────────────────────
            if (!await context.Books.AnyAsync())
            {
                var books = new List<(Book Book,
                    string[] Categories)>
                {
                    (new Book
                    {
                        Title = "The Great Gatsby",
                        Author = "F. Scott Fitzgerald",
                        ISBN = "978-0-7432-7356-5",
                        Description =
                            "A story of Jay Gatsby and his " +
                            "obsession with Daisy Buchanan " +
                            "set in the Jazz Age.",
                        PublishedYear = 1925,
                        Publisher = "Scribner",
                        TotalCopies = 5,
                        AvailableCopies = 5,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Fiction" }),

                    (new Book
                    {
                        Title = "To Kill a Mockingbird",
                        Author = "Harper Lee",
                        ISBN = "978-0-06-112008-4",
                        Description =
                            "A story of racial injustice " +
                            "and loss of innocence in the " +
                            "American South.",
                        PublishedYear = 1960,
                        Publisher = "HarperCollins",
                        TotalCopies = 4,
                        AvailableCopies = 4,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Fiction" }),

                    (new Book
                    {
                        Title = "1984",
                        Author = "George Orwell",
                        ISBN = "978-0-452-28423-4",
                        Description =
                            "A dystopian novel set in a " +
                            "totalitarian society controlled " +
                            "by Big Brother.",
                        PublishedYear = 1949,
                        Publisher = "Penguin Books",
                        TotalCopies = 6,
                        AvailableCopies = 6,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Fiction", "Science Fiction" }),

                    (new Book
                    {
                        Title = "A Brief History of Time",
                        Author = "Stephen Hawking",
                        ISBN = "978-0-553-38016-3",
                        Description =
                            "An exploration of cosmology " +
                            "covering the Big Bang and " +
                            "the nature of time.",
                        PublishedYear = 1988,
                        Publisher = "Bantam Books",
                        TotalCopies = 3,
                        AvailableCopies = 3,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Science" }),

                    (new Book
                    {
                        Title = "Clean Code",
                        Author = "Robert C. Martin",
                        ISBN = "978-0-13-235088-4",
                        Description =
                            "A handbook of agile software " +
                            "craftsmanship with practical " +
                            "advice for writing clean code.",
                        PublishedYear = 2008,
                        Publisher = "Prentice Hall",
                        TotalCopies = 4,
                        AvailableCopies = 4,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Technology" }),

                    (new Book
                    {
                        Title = "Sapiens",
                        Author = "Yuval Noah Harari",
                        ISBN = "978-0-06-231609-7",
                        Description =
                            "A narrative history about " +
                            "the most important developments " +
                            "in human history.",
                        PublishedYear = 2011,
                        Publisher = "Harper",
                        TotalCopies = 5,
                        AvailableCopies = 5,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "History", "Non-Fiction" }),

                    (new Book
                    {
                        Title = "The Da Vinci Code",
                        Author = "Dan Brown",
                        ISBN = "978-0-385-50420-5",
                        Description =
                            "Symbologist Robert Langdon " +
                            "investigates a murder in " +
                            "the Louvre museum.",
                        PublishedYear = 2003,
                        Publisher = "Doubleday",
                        TotalCopies = 5,
                        AvailableCopies = 5,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Mystery" }),

                    (new Book
                    {
                        Title = "Steve Jobs",
                        Author = "Walter Isaacson",
                        ISBN = "978-1-4516-4853-9",
                        Description =
                            "The exclusive biography of " +
                            "Apple co-founder Steve Jobs " +
                            "based on 40 plus interviews.",
                        PublishedYear = 2011,
                        Publisher = "Simon and Schuster",
                        TotalCopies = 3,
                        AvailableCopies = 3,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Biography" }),

                    (new Book
                    {
                        Title = "Atomic Habits",
                        Author = "James Clear",
                        ISBN = "978-0-7352-1129-2",
                        Description =
                            "A guide to building good " +
                            "habits and breaking bad ones " +
                            "using tiny changes.",
                        PublishedYear = 2018,
                        Publisher = "Avery",
                        TotalCopies = 6,
                        AvailableCopies = 6,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Self-Help" }),

                    (new Book
                    {
                        Title = "Harry Potter and the Philosophers Stone",
                        Author = "J.K. Rowling",
                        ISBN = "978-0-439-70818-8",
                        Description =
                            "A young boy discovers he is " +
                            "a wizard and begins his education " +
                            "at Hogwarts School.",
                        PublishedYear = 1997,
                        Publisher = "Bloomsbury",
                        TotalCopies = 8,
                        AvailableCopies = 8,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Fantasy", "Fiction" }),

                    (new Book
                    {
                        Title = "The Hobbit",
                        Author = "J.R.R. Tolkien",
                        ISBN = "978-0-618-00221-3",
                        Description =
                            "Bilbo Baggins goes on an " +
                            "unexpected journey to reclaim " +
                            "treasure from a dragon.",
                        PublishedYear = 1937,
                        Publisher = "Houghton Mifflin",
                        TotalCopies = 5,
                        AvailableCopies = 5,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Fantasy" }),

                    (new Book
                    {
                        Title = "The Martian",
                        Author = "Andy Weir",
                        ISBN = "978-0-553-41802-6",
                        Description =
                            "An astronaut is stranded on " +
                            "Mars and must use ingenuity " +
                            "to survive.",
                        PublishedYear = 2011,
                        Publisher = "Crown",
                        TotalCopies = 4,
                        AvailableCopies = 4,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Science Fiction" }),

                    (new Book
                    {
                        Title = "Thinking Fast and Slow",
                        Author = "Daniel Kahneman",
                        ISBN = "978-0-374-27563-1",
                        Description =
                            "Nobel Prize winner explores " +
                            "the two systems that drive " +
                            "the way we think.",
                        PublishedYear = 2011,
                        Publisher = "Farrar Straus",
                        TotalCopies = 4,
                        AvailableCopies = 4,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Non-Fiction" }),

                    (new Book
                    {
                        Title = "Rich Dad Poor Dad",
                        Author = "Robert Kiyosaki",
                        ISBN = "978-1-612-68098-3",
                        Description =
                            "What the rich teach their " +
                            "kids about money that the " +
                            "poor and middle class do not.",
                        PublishedYear = 1997,
                        Publisher = "Plata Publishing",
                        TotalCopies = 5,
                        AvailableCopies = 5,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Non-Fiction", "Self-Help" }),

                    (new Book
                    {
                        Title = "The Shining",
                        Author = "Stephen King",
                        ISBN = "978-0-385-12167-5",
                        Description =
                            "Jack Torrance becomes the " +
                            "winter caretaker of the " +
                            "Overlook Hotel with dark results.",
                        PublishedYear = 1977,
                        Publisher = "Doubleday",
                        TotalCopies = 4,
                        AvailableCopies = 4,
                        IsActive = true,
                        DateAdded = DateTime.Now
                    }, new[] { "Fiction", "Mystery" }),
                };

                // Add all books first
                foreach (var (book, _) in books)
                    context.Books.Add(book);
                await context.SaveChangesAsync();

                // Assign categories to books
                foreach (var (book, categoryNames) in books)
                {
                    foreach (var catName in categoryNames)
                    {
                        var cat = await context.Categories
                            .FirstOrDefaultAsync(c =>
                                c.Name == catName);
                        if (cat != null)
                        {
                            var exists = await context
                                .BookCategories
                                .AnyAsync(bc =>
                                    bc.BookId == book.Id &&
                                    bc.CategoryId == cat.Id);
                            if (!exists)
                            {
                                context.BookCategories.Add(
                                    new BookCategory
                                    {
                                        BookId = book.Id,
                                        CategoryId = cat.Id
                                    });
                            }
                        }
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }
}