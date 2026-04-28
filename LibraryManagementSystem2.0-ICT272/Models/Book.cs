using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Book Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;

        [StringLength(20)]
        public string ISBN { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Display(Name = "Published Year")]
        public int PublishedYear { get; set; }

        [StringLength(100)]
        public string? Publisher { get; set; }

        [Display(Name = "Total Copies")]
        [Range(1, 1000)]
        public int TotalCopies { get; set; }

        [Display(Name = "Available Copies")]
        public int AvailableCopies { get; set; }

        [Display(Name = "Cover Image")]
        public string? CoverImagePath { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; } = DateTime.Now;

        public ICollection<BookCategory> BookCategories { get; set; }
            = new List<BookCategory>();
        public ICollection<BorrowRecord> BorrowRecords { get; set; }
            = new List<BorrowRecord>();
        public ICollection<Reservation> Reservations { get; set; }
            = new List<Reservation>();
        public ICollection<Feedback> Feedbacks { get; set; }
            = new List<Feedback>();
    }
}