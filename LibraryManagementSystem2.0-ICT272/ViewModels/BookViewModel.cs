using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Book Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "ISBN")]
        public string ISBN { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Display(Name = "Published Year")]
        public int PublishedYear { get; set; }

        public string? Publisher { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Total Copies")]
        public int TotalCopies { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Cover Image")]
        public IFormFile? CoverImage { get; set; }

        public string? ExistingCoverImagePath { get; set; }

        public List<int> SelectedCategoryIds { get; set; }
            = new List<int>();

        public List<SelectListItem> AvailableCategories { get; set; }
            = new List<SelectListItem>();
    }
}