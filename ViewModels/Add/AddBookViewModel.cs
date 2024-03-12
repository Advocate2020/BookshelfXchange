using System.ComponentModel.DataAnnotations;

namespace BookshelfXchange.ViewModels.Add
{
    public class AddBookViewModel
    {
        [Required]
        [StringLength(150, ErrorMessage = "Title is too long")]
        public string Title { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "Author is too long")]
        public string Author { get; set; }

        [Required]
        [Range(1, 5000, ErrorMessage = "Category is missing")]
        public int CategoryId { get; set; }

    }
}
