using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        [StringLength(200)]
        public string? Address { get; set; }

        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; } = DateTime.Now;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        public ICollection<BorrowRecord> BorrowRecords { get; set; }
            = new List<BorrowRecord>();
        public ICollection<Reservation> Reservations { get; set; }
            = new List<Reservation>();
        public ICollection<Feedback> Feedbacks { get; set; }
            = new List<Feedback>();
    }
}