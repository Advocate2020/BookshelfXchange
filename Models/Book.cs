using System.ComponentModel.DataAnnotations.Schema;

namespace BookshelfXchange.Models
{

    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }


}