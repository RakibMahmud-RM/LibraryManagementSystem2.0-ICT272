using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public enum ReservationStatus
    {
        Pending,
        Fulfilled,
        Cancelled,
        Expired
    }

    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int BookId { get; set; }

        [Display(Name = "Reserved On")]
        public DateTime ReservationDate { get; set; } = DateTime.Now;

        [Display(Name = "Expires On")]
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddDays(7);

        [Display(Name = "Status")]
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public Book Book { get; set; } = null!;
    }
}