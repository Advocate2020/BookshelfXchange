using BookshelfXchange.ViewModels.GET;
using BookShelfXChange.Repository;
using Microsoft.AspNetCore.Components;

namespace BookShelfXChange.Components.Pages
{
    public partial class Index
    {
        private List<GetBookViewModel>? books;

        private IQueryable<GetBookViewModel>? bookItems;

        private bool IsLoading;

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        private IBaseRepository<GetBookViewModel> GetBookRepository { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;

                books = await GetBookRepository.GetAllAsync("api/Book");

                bookItems = books.AsQueryable();

                IsLoading = false;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while fetching books and categories: {ex.Message}");
            }

        }
    }
}