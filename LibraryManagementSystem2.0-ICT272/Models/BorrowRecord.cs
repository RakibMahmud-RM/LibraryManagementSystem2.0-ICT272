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

        [Required]
        public int BookId { get; set; }

        [Display(Name = "Borrow Date")]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Return Date")]
        public DateTime? ReturnDate { get; set; }

        [Display(Name = "Status")]
        public BorrowStatus Status { get; set; } = BorrowStatus.Borrowed;

        [Range(0, 10)]
        [Display(Name = "Renewals Used")]
        public int RenewalsUsed { get; set; } = 0;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public Book Book { get; set; } = null!;
        public Fine? Fine { get; set; }
    }
}