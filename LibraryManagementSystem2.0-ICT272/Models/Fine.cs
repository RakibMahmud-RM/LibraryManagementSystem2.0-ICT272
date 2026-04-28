using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class Fine
    {
        public int Id { get; set; }

        public int BorrowRecordId { get; set; }
        public BorrowRecord BorrowRecord { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [StringLength(200)]
        public string? Reason { get; set; }

        [Display(Name = "Issued Date")]
        public DateTime IssuedDate { get; set; } = DateTime.Now;

        [Display(Name = "Is Paid")]
        public bool IsPaid { get; set; } = false;

        public DateTime? PaidDate { get; set; }
    }
}