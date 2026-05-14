using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets — each one becomes a table in the database
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<BorrowingRule> BorrowingRules { get; set; }
        public DbSet<Fine> Fines { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<LibraryProfile> LibraryProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-many: Books ↔ Categories (composite primary key)
            modelBuilder.Entity<BookCategory>()
                .HasKey(bc => new { bc.BookId, bc.CategoryId });

            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Book)
                .WithMany(b => b.BookCategories)
                .HasForeignKey(bc => bc.BookId);

            modelBuilder.Entity<BookCategory>()
                .HasOne(bc => bc.Category)
                .WithMany(c => c.BookCategories)
                .HasForeignKey(bc => bc.CategoryId);

            // Fine → BorrowRecord (one-to-one)
            modelBuilder.Entity<Fine>()
                .HasOne(f => f.BorrowRecord)
                .WithOne(b => b.Fine)
                .HasForeignKey<Fine>(f => f.BorrowRecordId);

            // Decimal precision for Fine amount
            modelBuilder.Entity<Fine>()
                .Property(f => f.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<BorrowingRule>()
                .Property(r => r.OverduePenaltyPerDay)
                .HasColumnType("decimal(18,2)");
        }
    }
}