namespace BookshelfXchange.ViewModels.GET
{
    public class GetBookViewModel
    {

        public required int Id { get; set; }


        public required string Title { get; set; }

        public required string Author { get; set; }

        public required int CategoryId { get; set; }

        public required string CategoryName { get; set; }
    }
}
