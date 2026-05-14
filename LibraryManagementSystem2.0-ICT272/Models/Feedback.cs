using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, 5)]
        [Display(Name = "Rating (1-5)")]
        public int Rating { get; set; }

        [StringLength(1000)]
        [Display(Name = "Your Review")]
        public string? Comment { get; set; }

        [Display(Name = "Submitted On")]
        public DateTime SubmittedDate { get; set; } = DateTime.Now;

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; } = false;

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}