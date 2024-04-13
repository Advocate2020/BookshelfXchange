namespace BookShelfXChange.Models
{
    public class BookImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        // Other image properties...

        // Foreign key property for book relationship
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
