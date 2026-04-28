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
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        [Display(Name = "Reservation Date")]
        public DateTime ReservationDate { get; set; }
            = DateTime.Now;

        public ReservationStatus Status { get; set; }
            = ReservationStatus.Pending;
    }
}