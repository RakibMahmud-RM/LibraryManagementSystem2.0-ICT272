using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class LibraryProfile
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Library Name")]
        public string LibraryName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Location { get; set; }

        [StringLength(200)]
        [Display(Name = "Operating Hours")]
        public string? OperatingHours { get; set; }

        [StringLength(20)]
        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }

        [StringLength(100)]
        [Display(Name = "Contact Email")]
        public string? ContactEmail { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [Display(Name = "Established Year")]
        public int? EstablishedYear { get; set; }
    }
}