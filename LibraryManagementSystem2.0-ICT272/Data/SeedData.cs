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

            // ── Roles ─────────────────────────────────────────
            string[] roles = { "Admin", "Member" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
            }

            // ── Admin ─────────────────────────────────────────
            string adminEmail = "admin@library.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
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
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // ── Member ────────────────────────────────────────
            string memberEmail = "member@library.com";
            if (await userManager.FindByEmailAsync(memberEmail) == null)
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
                    await userManager.AddToRoleAsync(member, "Member");
            }

            // ── Categories ────────────────────────────────────
            if (!await context.Categories.AnyAsync())
            {
                var cats = new List<Category>
                {
                    new() { Name = "Fiction",
                        Description = "Fictional stories and novels" },
                    new() { Name = "Non-Fiction",
                        Description = "Real world facts and stories" },
                    new() { Name = "Science",
                        Description = "Scientific topics and research" },
                    new() { Name = "Technology",
                        Description = "Technology and computing" },
                    new() { Name = "History",
                        Description = "Historical events and figures" },
                    new() { Name = "Mystery",
                        Description = "Mystery and thriller novels" },
                    new() { Name = "Biography",
                        Description = "Life stories of real people" },
                    new() { Name = "Self-Help",
                        Description = "Personal development books" },
                    new() { Name = "Fantasy",
                        Description = "Fantasy and magical worlds" },
                    new() { Name = "Romance",
                        Description = "Love and relationship stories" },
                    new() { Name = "Horror",
                        Description = "Horror and scary stories" },
                    new() { Name = "Science Fiction",
                        Description = "Futuristic and space stories" },
                    new() { Name = "Children",
                        Description = "Books for children" },
                    new() { Name = "Psychology",
                        Description = "Human mind and behaviour" },
                    new() { Name = "Business",
                        Description = "Business and entrepreneurship" },
                    new() { Name = "Philosophy",
                        Description = "Philosophy and thinking" },
                    new() { Name = "Travel",
                        Description = "Travel and adventure" },
                    new() { Name = "Art",
                        Description = "Art and creativity" }
                };
                context.Categories.AddRange(cats);
                await context.SaveChangesAsync();
            }

            // ── Borrowing Rule ────────────────────────────────
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
                    Notes = "Default borrowing rule for all members"
                });
                await context.SaveChangesAsync();
            }

            // ── Library Profile ───────────────────────────────
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
                        "Your community library providing access " +
                        "to thousands of books and resources.",
                    EstablishedYear = 2005
                });
                await context.SaveChangesAsync();
            }

            // ── Books ─────────────────────────────────────────
            if (!await context.Books.AnyAsync())
            {
                var books = new List<(Book Book,
                    string[] CategoryNames)>
                {
                    // FICTION
                    (new Book { Title = "The Great Gatsby",
                        Author = "F. Scott Fitzgerald",
                        ISBN = "978-0-7432-7356-5",
                        Description = "A story of Jay Gatsby and " +
                            "his obsession with Daisy Buchanan " +
                            "set in the Jazz Age of the 1920s.",
                        PublishedYear = 1925,
                        Publisher = "Scribner",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780743273565-L.jpg" },
                        new[] { "Fiction" }),

                    (new Book { Title = "To Kill a Mockingbird",
                        Author = "Harper Lee",
                        ISBN = "978-0-06-112008-4",
                        Description = "The story of racial injustice " +
                            "and the loss of innocence in the " +
                            "American South during the 1930s.",
                        PublishedYear = 1960,
                        Publisher = "HarperCollins",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780061120084-L.jpg" },
                        new[] { "Fiction" }),

                    (new Book { Title = "1984",
                        Author = "George Orwell",
                        ISBN = "978-0-452-28423-4",
                        Description = "A dystopian novel set in " +
                            "a totalitarian society controlled " +
                            "by the mysterious Big Brother.",
                        PublishedYear = 1949,
                        Publisher = "Penguin Books",
                        TotalCopies = 6, AvailableCopies = 6,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780452284234-L.jpg" },
                        new[] { "Fiction", "Science Fiction" }),

                    (new Book { Title = "Animal Farm",
                        Author = "George Orwell",
                        ISBN = "978-0-452-28424-1",
                        Description = "A satirical allegorical " +
                            "novella reflecting events leading " +
                            "up to the Russian Revolution.",
                        PublishedYear = 1945,
                        Publisher = "Penguin Books",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780452284241-L.jpg" },
                        new[] { "Fiction" }),

                    (new Book { Title = "The Catcher in the Rye",
                        Author = "J.D. Salinger",
                        ISBN = "978-0-316-76948-0",
                        Description = "The story of Holden Caulfield " +
                            "a teenager from New York City expelled " +
                            "from prep school in Pennsylvania.",
                        PublishedYear = 1951,
                        Publisher = "Little Brown",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780316769488-L.jpg" },
                        new[] { "Fiction" }),

                    (new Book { Title = "Of Mice and Men",
                        Author = "John Steinbeck",
                        ISBN = "978-0-14-028726-7",
                        Description = "The story of two displaced " +
                            "migrant workers during the Great " +
                            "Depression in California.",
                        PublishedYear = 1937,
                        Publisher = "Penguin Books",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780140287264-L.jpg" },
                        new[] { "Fiction" }),

                    (new Book { Title = "Brave New World",
                        Author = "Aldous Huxley",
                        ISBN = "978-0-06-092987-7",
                        Description = "A futuristic society where " +
                            "people are engineered and conditioned " +
                            "for their predetermined roles.",
                        PublishedYear = 1932,
                        Publisher = "Harper Perennial",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780060929879-L.jpg" },
                        new[] { "Fiction", "Science Fiction" }),

                    (new Book { Title = "The Old Man and the Sea",
                        Author = "Ernest Hemingway",
                        ISBN = "978-0-684-80122-3",
                        Description = "An aging Cuban fisherman " +
                            "struggles with a giant marlin far " +
                            "out in the Gulf Stream.",
                        PublishedYear = 1952,
                        Publisher = "Scribner",
                        TotalCopies = 3, AvailableCopies = 3,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780684801223-L.jpg" },
                        new[] { "Fiction" }),

                    (new Book { Title = "Pride and Prejudice",
                        Author = "Jane Austen",
                        ISBN = "978-0-14-143951-8",
                        Description = "Elizabeth Bennet navigates " +
                            "issues of manners, upbringing, morality " +
                            "and marriage in Georgian-era England.",
                        PublishedYear = 1813,
                        Publisher = "Penguin Classics",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780141439518-L.jpg" },
                        new[] { "Fiction", "Romance" }),

                    (new Book { Title = "Wuthering Heights",
                        Author = "Emily Bronte",
                        ISBN = "978-0-14-143955-6",
                        Description = "The story of the intense " +
                            "and almost demonic love between " +
                            "Catherine Earnshaw and Heathcliff.",
                        PublishedYear = 1847,
                        Publisher = "Penguin Classics",
                        TotalCopies = 3, AvailableCopies = 3,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780141439556-L.jpg" },
                        new[] { "Fiction", "Romance" }),

                    // SCIENCE FICTION
                    (new Book { Title = "Dune",
                        Author = "Frank Herbert",
                        ISBN = "978-0-441-17271-9",
                        Description = "Set in the distant future " +
                            "amidst a feudal interstellar society " +
                            "in which various noble houses control " +
                            "planetary fiefs.",
                        PublishedYear = 1965,
                        Publisher = "Ace Books",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780441172719-L.jpg" },
                        new[] { "Science Fiction" }),

                    (new Book { Title = "The Hitchhikers Guide to the Galaxy",
                        Author = "Douglas Adams",
                        ISBN = "978-0-345-39180-3",
                        Description = "Earthman Arthur Dent is " +
                            "whisked away by his alien friend Ford " +
                            "Prefect just before Earth is demolished.",
                        PublishedYear = 1979,
                        Publisher = "Del Rey",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780345391803-L.jpg" },
                        new[] { "Science Fiction", "Fiction" }),

                    (new Book { Title = "Enders Game",
                        Author = "Orson Scott Card",
                        ISBN = "978-0-812-55070-5",
                        Description = "A young boy is recruited to " +
                            "attend a battle school in space to " +
                            "prepare for an alien invasion.",
                        PublishedYear = 1985,
                        Publisher = "Tor Books",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780812550702-L.jpg" },
                        new[] { "Science Fiction" }),

                    (new Book { Title = "The Martian",
                        Author = "Andy Weir",
                        ISBN = "978-0-553-41802-6",
                        Description = "An astronaut becomes stranded " +
                            "on Mars and must use his ingenuity to " +
                            "survive and find a way back to Earth.",
                        PublishedYear = 2011,
                        Publisher = "Crown",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780553418026-L.jpg" },
                        new[] { "Science Fiction" }),

                    (new Book { Title = "Neuromancer",
                        Author = "William Gibson",
                        ISBN = "978-0-441-56956-4",
                        Description = "A washed-up computer hacker " +
                            "is hired by a mysterious employer " +
                            "for one last job in cyberspace.",
                        PublishedYear = 1984,
                        Publisher = "Ace Books",
                        TotalCopies = 3, AvailableCopies = 3,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780441569564-L.jpg" },
                        new[] { "Science Fiction" }),

                    // FANTASY
                    (new Book { Title = "Harry Potter and the Philosophers Stone",
                        Author = "J.K. Rowling",
                        ISBN = "978-0-439-70818-8",
                        Description = "A young boy discovers he is " +
                            "a wizard and begins his education at " +
                            "Hogwarts School of Witchcraft.",
                        PublishedYear = 1997,
                        Publisher = "Bloomsbury",
                        TotalCopies = 8, AvailableCopies = 8,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780439708180-L.jpg" },
                        new[] { "Fantasy", "Fiction" }),

                    (new Book { Title = "Harry Potter and the Chamber of Secrets",
                        Author = "J.K. Rowling",
                        ISBN = "978-0-439-06486-6",
                        Description = "Harry returns to Hogwarts " +
                            "for his second year and discovers " +
                            "a dangerous secret in the school.",
                        PublishedYear = 1998,
                        Publisher = "Bloomsbury",
                        TotalCopies = 7, AvailableCopies = 7,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780439064866-L.jpg" },
                        new[] { "Fantasy", "Fiction" }),

                    (new Book { Title = "The Lord of the Rings Fellowship",
                        Author = "J.R.R. Tolkien",
                        ISBN = "978-0-618-57494-1",
                        Description = "A young hobbit Frodo Baggins " +
                            "sets out on a quest to destroy a " +
                            "powerful ring with a fellowship.",
                        PublishedYear = 1954,
                        Publisher = "Houghton Mifflin",
                        TotalCopies = 6, AvailableCopies = 6,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780618574940-L.jpg" },
                        new[] { "Fantasy" }),

                    (new Book { Title = "The Hobbit",
                        Author = "J.R.R. Tolkien",
                        ISBN = "978-0-618-00221-3",
                        Description = "Bilbo Baggins is swept into " +
                            "an epic quest to reclaim a lost dwarf " +
                            "kingdom from a dragon named Smaug.",
                        PublishedYear = 1937,
                        Publisher = "Houghton Mifflin",
                        TotalCopies = 6, AvailableCopies = 6,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780618002214-L.jpg" },
                        new[] { "Fantasy" }),

                    (new Book { Title = "A Game of Thrones",
                        Author = "George R.R. Martin",
                        ISBN = "978-0-553-57340-5",
                        Description = "Seven noble families fight " +
                            "for control of the mythical land " +
                            "of Westeros in a brutal power struggle.",
                        PublishedYear = 1996,
                        Publisher = "Bantam Books",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780553573404-L.jpg" },
                        new[] { "Fantasy" }),

                    (new Book { Title = "The Name of the Wind",
                        Author = "Patrick Rothfuss",
                        ISBN = "978-0-756-40407-1",
                        Description = "The story of Kvothe a " +
                            "legendary musician wizard and warrior " +
                            "told in his own words.",
                        PublishedYear = 2007,
                        Publisher = "DAW Books",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780756404079-L.jpg" },
                        new[] { "Fantasy" }),

                    // MYSTERY
                    (new Book { Title = "The Da Vinci Code",
                        Author = "Dan Brown",
                        ISBN = "978-0-385-50420-5",
                        Description = "Symbologist Robert Langdon " +
                            "investigates a murder in the Louvre " +
                            "uncovering a religious mystery.",
                        PublishedYear = 2003,
                        Publisher = "Doubleday",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780385504201-L.jpg" },
                        new[] { "Mystery" }),

                    (new Book { Title = "Angels and Demons",
                        Author = "Dan Brown",
                        ISBN = "978-0-743-49346-3",
                        Description = "Robert Langdon is called to " +
                            "investigate the murder of a physicist " +
                            "at CERN and discovers the Illuminati.",
                        PublishedYear = 2000,
                        Publisher = "Pocket Books",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780743493468-L.jpg" },
                        new[] { "Mystery" }),

                    (new Book { Title = "Gone Girl",
                        Author = "Gillian Flynn",
                        ISBN = "978-0-307-58836-4",
                        Description = "On their fifth wedding " +
                            "anniversary Nick Dunne reports that " +
                            "his wife Amy has gone missing.",
                        PublishedYear = 2012,
                        Publisher = "Crown",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780307588364-L.jpg" },
                        new[] { "Mystery" }),

                    (new Book { Title = "The Girl with the Dragon Tattoo",
                        Author = "Stieg Larsson",
                        ISBN = "978-0-307-45454-1",
                        Description = "A journalist and a young " +
                            "female hacker investigate a 40-year-old " +
                            "murder in a wealthy Swedish family.",
                        PublishedYear = 2005,
                        Publisher = "Norstedts Forlag",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780307454546-L.jpg" },
                        new[] { "Mystery" }),

                    (new Book { Title = "The Silent Patient",
                        Author = "Alex Michaelides",
                        ISBN = "978-1-250-30169-8",
                        Description = "A famous painter shoots her " +
                            "husband five times and then never " +
                            "speaks another word.",
                        PublishedYear = 2019,
                        Publisher = "Celadon Books",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9781250301697-L.jpg" },
                        new[] { "Mystery" }),

                    (new Book { Title = "Murder on the Orient Express",
                        Author = "Agatha Christie",
                        ISBN = "978-0-062-07397-5",
                        Description = "Hercule Poirot investigates " +
                            "the murder of an American tycoon " +
                            "on the famous Orient Express train.",
                        PublishedYear = 1934,
                        Publisher = "HarperCollins",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780062073976-L.jpg" },
                        new[] { "Mystery" }),

                    // SCIENCE
                    (new Book { Title = "A Brief History of Time",
                        Author = "Stephen Hawking",
                        ISBN = "978-0-553-38016-3",
                        Description = "An exploration of cosmology " +
                            "covering the Big Bang, black holes " +
                            "and the nature of time itself.",
                        PublishedYear = 1988,
                        Publisher = "Bantam Books",
                        TotalCopies = 3, AvailableCopies = 3,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780553380163-L.jpg" },
                        new[] { "Science" }),

                    (new Book { Title = "The Origin of Species",
                        Author = "Charles Darwin",
                        ISBN = "978-0-14-043205-3",
                        Description = "Darwin introduces the " +
                            "scientific theory of evolution by " +
                            "natural selection with evidence.",
                        PublishedYear = 1859,
                        Publisher = "Penguin Classics",
                        TotalCopies = 3, AvailableCopies = 3,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780140432053-L.jpg" },
                        new[] { "Science" }),

                    (new Book { Title = "The Selfish Gene",
                        Author = "Richard Dawkins",
                        ISBN = "978-0-199-57519-1",
                        Description = "Dawkins explains evolution " +
                            "from the perspective of the gene " +
                            "and introduces the concept of memes.",
                        PublishedYear = 1976,
                        Publisher = "Oxford University Press",
                        TotalCopies = 3, AvailableCopies = 3,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780199575190-L.jpg" },
                        new[] { "Science" }),

                    (new Book { Title = "Cosmos",
                        Author = "Carl Sagan",
                        ISBN = "978-0-345-53943-4",
                        Description = "An exploration of the " +
                            "universe and our place in it " +
                            "by the legendary astronomer.",
                        PublishedYear = 1980,
                        Publisher = "Ballantine Books",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780345539434-L.jpg" },
                        new[] { "Science" }),

                    (new Book { Title = "The Double Helix",
                        Author = "James D. Watson",
                        ISBN = "978-0-743-21630-3",
                        Description = "A personal account of the " +
                            "discovery of the structure of DNA " +
                            "by one of its discoverers.",
                        PublishedYear = 1968,
                        Publisher = "Touchstone",
                        TotalCopies = 3, AvailableCopies = 3,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780743216302-L.jpg" },
                        new[] { "Science" }),

                    // TECHNOLOGY
                    (new Book { Title = "Clean Code",
                        Author = "Robert C. Martin",
                        ISBN = "978-0-13-235088-4",
                        Description = "A handbook of agile software " +
                            "craftsmanship with practical advice " +
                            "for writing clean maintainable code.",
                        PublishedYear = 2008,
                        Publisher = "Prentice Hall",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780132350884-L.jpg" },
                        new[] { "Technology" }),

                    (new Book { Title = "The Pragmatic Programmer",
                        Author = "Andrew Hunt",
                        ISBN = "978-0-201-61622-4",
                        Description = "A guide to becoming a better " +
                            "programmer by learning to solve problems " +
                            "in creative and pragmatic ways.",
                        PublishedYear = 1999,
                        Publisher = "Addison-Wesley",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780201616224-L.jpg" },
                        new[] { "Technology" }),

                    (new Book { Title = "Design Patterns",
                        Author = "Gang of Four",
                        ISBN = "978-0-201-63361-0",
                        Description = "Elements of reusable " +
                            "object-oriented software covering " +
                            "23 classic design patterns.",
                        PublishedYear = 1994,
                        Publisher = "Addison-Wesley",
                        TotalCopies = 3, AvailableCopies = 3,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780201633610-L.jpg" },
                        new[] { "Technology" }),

                    (new Book { Title = "The Lean Startup",
                        Author = "Eric Ries",
                        ISBN = "978-0-307-88789-4",
                        Description = "How today's entrepreneurs use " +
                            "continuous innovation to create " +
                            "radically successful businesses.",
                        PublishedYear = 2011,
                        Publisher = "Crown Business",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780307887894-L.jpg" },
                        new[] { "Technology", "Business" }),

                    (new Book { Title = "Zero to One",
                        Author = "Peter Thiel",
                        ISBN = "978-0-804-13929-8",
                        Description = "Notes on startups or how " +
                            "to build the future by PayPal " +
                            "co-founder Peter Thiel.",
                        PublishedYear = 2014,
                        Publisher = "Crown Business",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780804139298-L.jpg" },
                        new[] { "Technology", "Business" }),

                    (new Book { Title = "Artificial Intelligence A Guide",
                        Author = "Melanie Mitchell",
                        ISBN = "978-0-374-25783-5",
                        Description = "A clear-eyed look at what AI " +
                            "is doing today what it cannot do and " +
                            "what its future may hold.",
                        PublishedYear = 2019,
                        Publisher = "Farrar Straus and Giroux",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780374257835-L.jpg" },
                        new[] { "Technology", "Science" }),

                    // BIOGRAPHY
                    (new Book { Title = "Steve Jobs",
                        Author = "Walter Isaacson",
                        ISBN = "978-1-4516-4853-9",
                        Description = "The exclusive biography based " +
                            "on more than forty interviews with Jobs " +
                            "himself conducted over two years.",
                        PublishedYear = 2011,
                        Publisher = "Simon and Schuster",
                        TotalCopies = 3, AvailableCopies = 3,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9781451648539-L.jpg" },
                        new[] { "Biography" }),

                    (new Book { Title = "Elon Musk",
                        Author = "Walter Isaacson",
                        ISBN = "978-1-982-18199-1",
                        Description = "An intimate biography of the " +
                            "most daring and riveting entrepreneur " +
                            "of our time Elon Musk.",
                        PublishedYear = 2023,
                        Publisher = "Simon and Schuster",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9781982181994-L.jpg" },
                        new[] { "Biography" }),

                    (new Book { Title = "The Diary of a Young Girl",
                        Author = "Anne Frank",
                        ISBN = "978-0-553-29698-1",
                        Description = "The diary written by Jewish " +
                            "girl Anne Frank while in hiding during " +
                            "the Nazi occupation of the Netherlands.",
                        PublishedYear = 1947,
                        Publisher = "Bantam Books",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780553296983-L.jpg" },
                        new[] { "Biography", "History" }),

                    (new Book { Title = "Long Walk to Freedom",
                        Author = "Nelson Mandela",
                        ISBN = "978-0-316-54818-3",
                        Description = "The autobiography of South " +
                            "African anti-apartheid revolutionary " +
                            "and president Nelson Mandela.",
                        PublishedYear = 1994,
                        Publisher = "Little Brown",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780316548182-L.jpg" },
                        new[] { "Biography", "History" }),

                    (new Book { Title = "Leonardo da Vinci",
                        Author = "Walter Isaacson",
                        ISBN = "978-1-501-12891-8",
                        Description = "A biography of history's " +
                            "most creative genius based on thousands " +
                            "of pages from his notebooks.",
                        PublishedYear = 2017,
                        Publisher = "Simon and Schuster",
                        TotalCopies = 3, AvailableCopies = 3,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9781501128912-L.jpg" },
                        new[] { "Biography", "Art" }),

                    // SELF-HELP
                    (new Book { Title = "Atomic Habits",
                        Author = "James Clear",
                        ISBN = "978-0-7352-1129-2",
                        Description = "A guide to building good habits " +
                            "and breaking bad ones using tiny changes " +
                            "that deliver remarkable results.",
                        PublishedYear = 2018,
                        Publisher = "Avery",
                        TotalCopies = 6, AvailableCopies = 6,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780735211292-L.jpg" },
                        new[] { "Self-Help" }),

                    (new Book { Title = "The 7 Habits of Highly Effective People",
                        Author = "Stephen Covey",
                        ISBN = "978-0-743-26951-3",
                        Description = "Powerful lessons in personal " +
                            "change offering a holistic integrated " +
                            "approach to solving problems.",
                        PublishedYear = 1989,
                        Publisher = "Free Press",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780743269513-L.jpg" },
                        new[] { "Self-Help" }),

                    (new Book { Title = "Think and Grow Rich",
                        Author = "Napoleon Hill",
                        ISBN = "978-1-585-42433-3",
                        Description = "A personal development and " +
                            "self-improvement book studying the " +
                            "habits of wealthy individuals.",
                        PublishedYear = 1937,
                        Publisher = "Tarcher Perigee",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9781585424337-L.jpg" },
                        new[] { "Self-Help" }),

                    (new Book { Title = "How to Win Friends and Influence People",
                        Author = "Dale Carnegie",
                        ISBN = "978-0-671-02703-5",
                        Description = "A classic self-help book that " +
                            "has sold over 30 million copies teaching " +
                            "fundamental techniques in handling people.",
                        PublishedYear = 1936,
                        Publisher = "Simon and Schuster",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780671027032-L.jpg" },
                        new[] { "Self-Help" }),

                    (new Book { Title = "The Power of Now",
                        Author = "Eckhart Tolle",
                        ISBN = "978-1-577-31480-6",
                        Description = "A guide to spiritual " +
                            "enlightenment focusing on the importance " +
                            "of living in the present moment.",
                        PublishedYear = 1997,
                        Publisher = "New World Library",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9781577314806-L.jpg" },
                        new[] { "Self-Help" }),

                    (new Book { Title = "Mans Search for Meaning",
                        Author = "Viktor Frankl",
                        ISBN = "978-0-807-01427-1",
                        Description = "A Holocaust survivor describes " +
                            "life in Nazi concentration camps and " +
                            "the importance of finding meaning.",
                        PublishedYear = 1946,
                        Publisher = "Beacon Press",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780807014271-L.jpg" },
                        new[] { "Self-Help", "Biography" }),

                    // HISTORY
                    (new Book { Title = "Sapiens A Brief History of Humankind",
                        Author = "Yuval Noah Harari",
                        ISBN = "978-0-06-231609-7",
                        Description = "A narrative history about the " +
                            "most important developments in human " +
                            "history from the Stone Age onwards.",
                        PublishedYear = 2011,
                        Publisher = "Harper",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780062316097-L.jpg" },
                        new[] { "History", "Non-Fiction" }),

                    (new Book { Title = "Homo Deus",
                        Author = "Yuval Noah Harari",
                        ISBN = "978-0-06-246431-6",
                        Description = "A look at tomorrow's possible " +
                            "developments in the fields of science " +
                            "technology and society.",
                        PublishedYear = 2015,
                        Publisher = "Harper",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780062464316-L.jpg" },
                        new[] { "History", "Non-Fiction" }),

                    (new Book { Title = "The Art of War",
                        Author = "Sun Tzu",
                        ISBN = "978-1-590-30225-5",
                        Description = "An ancient Chinese military " +
                            "treatise dating from the 5th century BC " +
                            "attributed to Sun Tzu.",
                        PublishedYear = 500,
                        Publisher = "Shambhala",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9781590302255-L.jpg" },
                        new[] { "History", "Philosophy" }),

                    (new Book { Title = "Guns Germs and Steel",
                        Author = "Jared Diamond",
                        ISBN = "978-0-393-31755-8",
                        Description = "An examination of why Western " +
                            "civilizations have come to dominate the " +
                            "world through history.",
                        PublishedYear = 1997,
                        Publisher = "W W Norton",
                        TotalCopies = 3, AvailableCopies = 3,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780393317558-L.jpg" },
                        new[] { "History" }),

                    // BUSINESS
                    (new Book { Title = "Good to Great",
                        Author = "Jim Collins",
                        ISBN = "978-0-066-62099-5",
                        Description = "Why some companies make the " +
                            "leap to greatness and others do not " +
                            "based on a five-year research study.",
                        PublishedYear = 2001,
                        Publisher = "HarperBusiness",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780066620992-L.jpg" },
                        new[] { "Business" }),

                    (new Book { Title = "Rich Dad Poor Dad",
                        Author = "Robert Kiyosaki",
                        ISBN = "978-1-612-68098-3",
                        Description = "What the rich teach their " +
                            "kids about money that the poor and " +
                            "middle class do not.",
                        PublishedYear = 1997,
                        Publisher = "Plata Publishing",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9781612680989-L.jpg" },
                        new[] { "Business", "Self-Help" }),

                    (new Book { Title = "The Intelligent Investor",
                        Author = "Benjamin Graham",
                        ISBN = "978-0-060-55566-5",
                        Description = "The classic text on value " +
                            "investing widely considered to be the " +
                            "best book on investing ever written.",
                        PublishedYear = 1949,
                        Publisher = "HarperBusiness",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780060555665-L.jpg" },
                        new[] { "Business" }),

                    (new Book { Title = "Start with Why",
                        Author = "Simon Sinek",
                        ISBN = "978-1-591-84276-5",
                        Description = "How great leaders inspire " +
                            "everyone to take action by starting " +
                            "with the question of why.",
                        PublishedYear = 2009,
                        Publisher = "Portfolio",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9781591842767-L.jpg" },
                        new[] { "Business", "Self-Help" }),

                    // PSYCHOLOGY
                    (new Book { Title = "Thinking Fast and Slow",
                        Author = "Daniel Kahneman",
                        ISBN = "978-0-374-27563-1",
                        Description = "Nobel Prize winner Kahneman " +
                            "explores the two systems that drive " +
                            "the way we think and make choices.",
                        PublishedYear = 2011,
                        Publisher = "Farrar Straus and Giroux",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780374275631-L.jpg" },
                        new[] { "Psychology" }),

                    (new Book { Title = "Influence The Psychology of Persuasion",
                        Author = "Robert Cialdini",
                        ISBN = "978-0-061-76344-8",
                        Description = "An examination of the " +
                            "psychology of compliance and what " +
                            "makes people say yes to requests.",
                        PublishedYear = 1984,
                        Publisher = "Harper Business",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780061763441-L.jpg" },
                        new[] { "Psychology" }),

                    (new Book { Title = "The Psychology of Money",
                        Author = "Morgan Housel",
                        ISBN = "978-0-857-19776-9",
                        Description = "Timeless lessons on wealth " +
                            "greed and happiness exploring how " +
                            "people think about money.",
                        PublishedYear = 2020,
                        Publisher = "Harriman House",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780857197689-L.jpg" },
                        new[] { "Psychology", "Business" }),

                    (new Book { Title = "Emotional Intelligence",
                        Author = "Daniel Goleman",
                        ISBN = "978-0-553-37506-9",
                        Description = "Why it can matter more than " +
                            "IQ exploring the role of emotional " +
                            "intelligence in success.",
                        PublishedYear = 1995,
                        Publisher = "Bantam Books",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780553375060-L.jpg" },
                        new[] { "Psychology" }),

                    // PHILOSOPHY
                    (new Book { Title = "Meditations",
                        Author = "Marcus Aurelius",
                        ISBN = "978-0-812-96843-7",
                        Description = "A series of personal writings " +
                            "by Roman Emperor Marcus Aurelius " +
                            "reflecting Stoic philosophy.",
                        PublishedYear = 180,
                        Publisher = "Modern Library",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780812968437-L.jpg" },
                        new[] { "Philosophy" }),

                    (new Book { Title = "The Republic",
                        Author = "Plato",
                        ISBN = "978-0-872-20579-9",
                        Description = "Platos masterwork covering " +
                            "justice, the nature of political rule " +
                            "and the concept of the ideal state.",
                        PublishedYear = 380,
                        Publisher = "Hackett Publishing",
                        TotalCopies = 3, AvailableCopies = 3,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780872205796-L.jpg" },
                        new[] { "Philosophy" }),

                    (new Book { Title = "Nicomachean Ethics",
                        Author = "Aristotle",
                        ISBN = "978-0-872-20464-8",
                        Description = "Aristotles best known work " +
                            "on ethics covering the nature of " +
                            "happiness and virtue.",
                        PublishedYear = 350,
                        Publisher = "Hackett Publishing",
                        TotalCopies = 3, AvailableCopies = 3,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780872204645-L.jpg" },
                        new[] { "Philosophy" }),

                    // HORROR
                    (new Book { Title = "It",
                        Author = "Stephen King",
                        ISBN = "978-1-501-14224-2",
                        Description = "Seven adults return to their " +
                            "hometown to confront a murderous " +
                            "supernatural force called Pennywise.",
                        PublishedYear = 1986,
                        Publisher = "Scribner",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9781501142246-L.jpg" },
                        new[] { "Horror" }),

                    (new Book { Title = "The Shining",
                        Author = "Stephen King",
                        ISBN = "978-0-385-12167-5",
                        Description = "Jack Torrance becomes the " +
                            "winter caretaker of the Overlook Hotel " +
                            "isolated in the Colorado mountains.",
                        PublishedYear = 1977,
                        Publisher = "Doubleday",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780385121675-L.jpg" },
                        new[] { "Horror" }),

                    (new Book { Title = "Dracula",
                        Author = "Bram Stoker",
                        ISBN = "978-0-141-43984-6",
                        Description = "The classic horror novel " +
                            "introducing Count Dracula and his " +
                            "attempts to move to England.",
                        PublishedYear = 1897,
                        Publisher = "Penguin Classics",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780141439846-L.jpg" },
                        new[] { "Horror" }),

                    (new Book { Title = "Frankenstein",
                        Author = "Mary Shelley",
                        ISBN = "978-0-141-43947-1",
                        Description = "Victor Frankenstein creates " +
                            "a living creature from dead tissue " +
                            "with disastrous consequences.",
                        PublishedYear = 1818,
                        Publisher = "Penguin Classics",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780141439471-L.jpg" },
                        new[] { "Horror", "Science Fiction" }),

                    // ROMANCE
                    (new Book { Title = "The Notebook",
                        Author = "Nicholas Sparks",
                        ISBN = "978-0-446-60521-0",
                        Description = "The timeless story of a " +
                            "poor young man who falls in love " +
                            "with a rich young woman in the 1940s.",
                        PublishedYear = 1996,
                        Publisher = "Warner Books",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780446605212-L.jpg" },
                        new[] { "Romance" }),

                    (new Book { Title = "Outlander",
                        Author = "Diana Gabaldon",
                        ISBN = "978-0-440-21256-1",
                        Description = "A British combat nurse from " +
                            "1945 is mysteriously swept back in " +
                            "time to 1743 Scotland.",
                        PublishedYear = 1991,
                        Publisher = "Delacorte Press",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780440212560-L.jpg" },
                        new[] { "Romance", "History" }),

                    // CHILDREN
                    (new Book { Title = "Charlottes Web",
                        Author = "E.B. White",
                        ISBN = "978-0-064-40055-8",
                        Description = "The story of Wilbur the pig " +
                            "and his friendship with a barn spider " +
                            "named Charlotte who saves his life.",
                        PublishedYear = 1952,
                        Publisher = "HarperCollins",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780064400558-L.jpg" },
                        new[] { "Children" }),

                    (new Book { Title = "The Very Hungry Caterpillar",
                        Author = "Eric Carle",
                        ISBN = "978-0-399-22690-8",
                        Description = "A caterpillar eats through " +
                            "a variety of foods before transforming " +
                            "into a beautiful butterfly.",
                        PublishedYear = 1969,
                        Publisher = "Philomel Books",
                        TotalCopies = 6, AvailableCopies = 6,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780399226908-L.jpg" },
                        new[] { "Children" }),

                    (new Book { Title = "Matilda",
                        Author = "Roald Dahl",
                        ISBN = "978-0-142-41037-1",
                        Description = "Matilda is a little girl " +
                            "who has extraordinary powers and " +
                            "gets revenge on cruel adults.",
                        PublishedYear = 1988,
                        Publisher = "Puffin Books",
                        TotalCopies = 6, AvailableCopies = 6,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780142410370-L.jpg" },
                        new[] { "Children", "Fiction" }),

                    (new Book { Title = "The Lion the Witch and the Wardrobe",
                        Author = "C.S. Lewis",
                        ISBN = "978-0-064-40494-5",
                        Description = "Four children discover a " +
                            "magic wardrobe that leads to the " +
                            "land of Narnia ruled by an evil witch.",
                        PublishedYear = 1950,
                        Publisher = "HarperCollins",
                        TotalCopies = 6, AvailableCopies = 6,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780064404945-L.jpg" },
                        new[] { "Children", "Fantasy" }),

                    // NON-FICTION
                    (new Book { Title = "Educated",
                        Author = "Tara Westover",
                        ISBN = "978-0-399-59050-4",
                        Description = "A memoir about a young woman " +
                            "born to survivalist parents who goes " +
                            "on to earn a PhD from Cambridge.",
                        PublishedYear = 2018,
                        Publisher = "Random House",
                        TotalCopies = 5, AvailableCopies = 5,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780399590504-L.jpg" },
                        new[] { "Non-Fiction", "Biography" }),

                    (new Book { Title = "The Immortal Life of Henrietta Lacks",
                        Author = "Rebecca Skloot",
                        ISBN = "978-1-400-05218-9",
                        Description = "The story of Henrietta Lacks " +
                            "whose cancer cells became one of the " +
                            "most important tools in medicine.",
                        PublishedYear = 2010,
                        Publisher = "Crown",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9781400052189-L.jpg" },
                        new[] { "Non-Fiction", "Science" }),

                    (new Book { Title = "In Cold Blood",
                        Author = "Truman Capote",
                        ISBN = "978-0-679-74558-2",
                        Description = "A true crime book detailing " +
                            "the 1959 murders of the Clutter family " +
                            "in Holcomb Kansas.",
                        PublishedYear = 1966,
                        Publisher = "Random House",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780679745587-L.jpg" },
                        new[] { "Non-Fiction" }),

                    (new Book { Title = "Freakonomics",
                        Author = "Steven D. Levitt",
                        ISBN = "978-0-060-73132-4",
                        Description = "A rogue economist explores " +
                            "the hidden side of everything using " +
                            "economic analysis in surprising ways.",
                        PublishedYear = 2005,
                        Publisher = "William Morrow",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780060731328-L.jpg" },
                        new[] { "Non-Fiction" }),

                    (new Book { Title = "The Tipping Point",
                        Author = "Malcolm Gladwell",
                        ISBN = "978-0-316-34662-7",
                        Description = "How little things can make " +
                            "a big difference exploring what makes " +
                            "social epidemics tip.",
                        PublishedYear = 2000,
                        Publisher = "Little Brown",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780316346627-L.jpg" },
                        new[] { "Non-Fiction" }),

                    (new Book { Title = "Outliers",
                        Author = "Malcolm Gladwell",
                        ISBN = "978-0-316-01792-3",
                        Description = "The story of success " +
                            "examining why some people achieve " +
                            "so much more than others.",
                        PublishedYear = 2008,
                        Publisher = "Little Brown",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780316017923-L.jpg" },
                        new[] { "Non-Fiction" }),

                    // TRAVEL
                    (new Book { Title = "Into the Wild",
                        Author = "Jon Krakauer",
                        ISBN = "978-0-385-48680-4",
                        Description = "The story of Christopher " +
                            "McCandless who walked into the " +
                            "Alaskan wilderness and never returned.",
                        PublishedYear = 1996,
                        Publisher = "Anchor Books",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780385486804-L.jpg" },
                        new[] { "Travel", "Non-Fiction" }),

                    (new Book { Title = "Eat Pray Love",
                        Author = "Elizabeth Gilbert",
                        ISBN = "978-0-143-03841-1",
                        Description = "A womans journey across " +
                            "Italy India and Indonesia as she " +
                            "seeks self-discovery after divorce.",
                        PublishedYear = 2006,
                        Publisher = "Penguin Books",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780143038412-L.jpg" },
                        new[] { "Travel", "Non-Fiction" }),

                    (new Book { Title = "Wild",
                        Author = "Cheryl Strayed",
                        ISBN = "978-0-307-47233-0",
                        Description = "A solo hike of over 1000 " +
                            "miles along the Pacific Crest Trail " +
                            "as a journey of self-discovery.",
                        PublishedYear = 2012,
                        Publisher = "Knopf",
                        TotalCopies = 4, AvailableCopies = 4,
                        IsActive = true, DateAdded = DateTime.Now,
                        CoverImagePath = "https://covers.openlibrary.org/b/isbn/9780307472335-L.jpg" },
                        new[] { "Travel", "Non-Fiction" }),
                };

                // Add all books first
                foreach (var (book, _) in books)
                    context.Books.Add(book);
                await context.SaveChangesAsync();

                // Now assign categories
                foreach (var (book, categoryNames) in books)
                {
                    foreach (var catName in categoryNames)
                    {
                        var cat = await context.Categories
                            .FirstOrDefaultAsync(c =>
                                c.Name == catName);
                        if (cat != null)
                        {
                            var exists = await context.BookCategories
                                .AnyAsync(bc =>
                                    bc.BookId == book.Id
                                    && bc.CategoryId == cat.Id);
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