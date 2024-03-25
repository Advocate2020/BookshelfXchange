using Blazored.Toast.Services;
using BookshelfXchange.Repository;
using BookshelfXchange.ViewModels.GET;
using BookshelfXchange.ViewModels.POST;
using Microsoft.AspNetCore.Components;

namespace BookshelfXchange.Components.Pages.Admin_Pages.books
{
    public partial class CreateBook
    {

        [SupplyParameterFromForm]
        private AddBookViewModel BookModel { get; set; } = new();

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private bool Success { get; set; }

        [Inject]
        private IBaseRepository<GetCategoryViewModel> CategoryRepository { get; set; }

        [Inject]
        private IBaseRepository<AddBookViewModel> BookRepository { get; set; }

        private List<GetCategoryViewModel>? BookCategories;

        int selectedCategoryID;

        int SelectedCategoryID
        {
            get => selectedCategoryID;
            set { selectedCategoryID = value; }
        }

        private bool isProcessing = false;

        protected override async Task OnInitializedAsync()
        {
            // Simulate asynchronous loading to demonstrate streaming rendering


            try
            {
                BookCategories = await CategoryRepository.GetAllAsync("api/Category");
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                toastService.ShowToast(ToastLevel.Error, $"Error occurred while fetching categories: {ex.Message}");
            }
        }


        private async Task Create()
        {
            try
            {
                isProcessing = true;
                // Call service method to add the book
                Success = await BookRepository.AddAsync(BookModel, "api/Book", null);


                if (Success)
                {
                    await Task.Delay(5000);
                    isProcessing = false;

                    toastService.ShowToast(ToastLevel.Success, "Book Added successfully");


                    BookModel = new AddBookViewModel();

                    //await Task.Delay(5000);

                    //NavigationManager.NavigateTo("/books");
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