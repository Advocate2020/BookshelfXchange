using Blazored.Toast.Services;
using BookshelfXchange.Repository;
using BookshelfXchange.ViewModels.Add;
using BookshelfXchange.ViewModels.GET;
using Microsoft.AspNetCore.Components;

namespace BookshelfXchange.Components.Pages.books
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
                Console.WriteLine($"Error occurred while fetching categories: {ex.Message}");
            }
        }


        private async Task Create()
        {
            // You can perform additional validation here if needed


            try
            {
                // Call service method to add the book
                Success = await BookRepository.AddAsync(BookModel, "api/Book");


                if (Success)
                {
                    toastService.ShowToast(ToastLevel.Success, "Book Added successfully");


                    BookModel = new AddBookViewModel();

                    //await Task.Delay(5000);

                    //NavigationManager.NavigateTo("/books");
                }


            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error occurred while adding the book: {ex.Message}");

                // Optionally, show an error message to the user
            }


        }



    }
}