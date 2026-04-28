using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public enum BorrowStatus
    {
        Borrowed,
        Returned,
        Overdue,
        Renewed
    }

    public class BorrowRecord
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        [Display(Name = "Borrow Date")]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Return Date")]
        public DateTime? ReturnDate { get; set; }

        public BorrowStatus Status { get; set; }
            = BorrowStatus.Borrowed;

        [Display(Name = "Renewal Count")]
        public int RenewalCount { get; set; } = 0;

        public string? Notes { get; set; }

        public Fine? Fine { get; set; }
    }
}