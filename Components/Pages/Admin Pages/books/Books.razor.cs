using Blazored.Toast.Services;
using BookshelfXchange.ViewModels.GET;
using BookShelfXChange.Repository;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;


namespace BookShelfXChange.Components.Pages.Admin_Pages.books
{
    public partial class Books
    {
        private List<GetBookViewModel>? books;

        private IQueryable<GetBookViewModel>? bookItems;

        private bool IsLoading;

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        private IBaseRepository<GetBookViewModel> GetBookRepository { get; set; }

        private PaginationState PaginationState { get; set; }

        private string titleFilter { get; set; }

        private bool Success
        {
            get; set;
        }

        IQueryable<GetBookViewModel> FilteredBookTitle
        {
            get
            {
                var result = bookItems;

                if (!string.IsNullOrEmpty(titleFilter))
                {
                    result = result.Where((book)
                           => book.Title.Contains(titleFilter, StringComparison.CurrentCultureIgnoreCase)
                           || book.CategoryName.Contains(titleFilter, StringComparison.CurrentCultureIgnoreCase)
                           || book.Author.Contains(titleFilter, StringComparison.CurrentCultureIgnoreCase));
                }

                return result;
            }
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;



                PaginationState = new PaginationState { ItemsPerPage = 5 };

                books = await GetBookRepository.GetAllAsync("api/Book");

                bookItems = books.AsQueryable();

                IsLoading = false;

            }
            catch (Exception ex)
            {
                toastService.ShowToast(ToastLevel.Error, $"Error occurred while fetching books and categories: {ex.Message}");
            }

        }

        void Update(GetBookViewModel vm)
        {
            NavigationManager.NavigateTo($"/book/edit/{vm.Id}");
        }
        private async Task Delete(GetBookViewModel vm)
        {
            try
            {
                // Call service method to add the book
                Success = await GetBookRepository.DeleteAsync("api/Book", vm.Id);


                if (Success)
                {
                    toastService.ShowToast(ToastLevel.Success, "Book deleted successfully");

                    await Task.Delay(5000);

                    NavigationManager.NavigateTo("/books", forceLoad: true); //forcedLoad, do i need it??
                }


            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                toastService.ShowToast(ToastLevel.Error, $"Error occurred while deleting the book: {ex.Message}");

                // Optionally, show an error message to the user
            }
        }

    }
}