namespace BookshelfXchange.Components.Layout
{
    public partial class LandingLayout
    {
        private bool IsLoggedIn { get; set; }
        protected override async Task OnInitializedAsync()
        {
            // Simulate asynchronous loading to demonstrate streaming rendering
            //try
            //{
            //    IsLoggedIn = await CategoryRepository.GetAllAsync("api/Category");
            //}
            //catch (Exception ex)
            //{
            //    // Log or handle the exception as needed
            //    Console.WriteLine($"Error occurred: {ex.Message}");
            //}
        }
    }
}