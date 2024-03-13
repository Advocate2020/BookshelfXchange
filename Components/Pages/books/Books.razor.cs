using Blazored.Toast.Services;
using BookshelfXchange.Repository;
using BookshelfXchange.ViewModels.GET;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.QuickGrid;


namespace BookshelfXchange.Components.Pages.books
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

                await Task.Delay(500);

                PaginationState = new PaginationState { ItemsPerPage = 5 };

                books = await GetBookRepository.GetAllAsync("api/Book");

                bookItems = books.AsQueryable();

            }
            catch (Exception ex)
            {
                toastService.ShowToast(ToastLevel.Error, $"Error occurred while fetching books and categories: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        void Update(GetBookViewModel viewModel)
        {
            NavigationManager.NavigateTo($"/book/edit/{viewModel.Id}");
        }
        private async Task Delete(GetBookViewModel viewModel)
        {
            try
            {
                // Call service method to add the book
                Success = await GetBookRepository.DeleteAsync("api/Book", viewModel.Id);


                if (Success)
                {
                    toastService.ShowToast(ToastLevel.Success, "Book updated successfully");

                    await Task.Delay(5000);

                    NavigationManager.NavigateTo("/books", forceLoad: true); //forcedLoad, do i need it??
                }


            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                toastService.ShowToast(ToastLevel.Error, $"Error occurred while adding the book: {ex.Message}");

                // Optionally, show an error message to the user
            }
        }

    }
}