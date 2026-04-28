using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class BorrowingRule
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Rule Name")]
        public string RuleName { get; set; } = string.Empty;

        [Display(Name = "Loan Duration (Days)")]
        [Range(1, 365)]
        public int LoanDurationDays { get; set; } = 14;

        [Display(Name = "Max Renewals")]
        [Range(0, 10)]
        public int MaxRenewals { get; set; } = 2;

        [Display(Name = "Penalty Per Day ($)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OverduePenaltyPerDay { get; set; } = 0.50m;

        [Display(Name = "Max Borrowable Items")]
        [Range(1, 20)]
        public int MaxBorrowableItems { get; set; } = 5;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public string? Notes { get; set; }
    }
}