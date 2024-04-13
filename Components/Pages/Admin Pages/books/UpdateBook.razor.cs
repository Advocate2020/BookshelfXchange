using AutoMapper;
using Blazored.Toast.Services;
using BookshelfXchange.ViewModels.GET;
using BookshelfXchange.ViewModels.Update;
using BookShelfXChange.Repository;
using Microsoft.AspNetCore.Components;

namespace BookShelfXChange.Components.Pages.Admin_Pages.books
{
    public partial class UpdateBook
    {
        [Parameter]
        public string BookId { get; set; }

        public UpdateBookViewModel _viewModel { get; set; } = new UpdateBookViewModel();
        private GetBookViewModel _model { get; set; }

        [Inject]
        private IBaseRepository<GetCategoryViewModel> CategoryRepository { get; set; }

        [Inject]
        private IBaseRepository<GetBookViewModel> BookRepository { get; set; }

        [Inject]
        private IBaseRepository<UpdateBookViewModel> UpdateBookRepository { get; set; }

        [Inject]
        public IMapper Mapper { get; set; }

        private List<GetCategoryViewModel>? BookCategories;

        [Inject]
        NavigationManager NavigationManager { get; set; }

        private bool Success
        {
            get; set;
        }

        protected override async Task OnInitializedAsync()
        {
            // Simulate asynchronous loading to demonstrate streaming rendering
            try
            {
                _model = await BookRepository.GetByIdAsync(BookId, "api/Book");

                Mapper.Map(_model, _viewModel);

                BookCategories = await CategoryRepository.GetAllAsync("api/Category");
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                toastService.ShowToast(ToastLevel.Error, $"Error occurred while fetching categories: {ex.Message}");
            }
        }

        string GetCatoryName(int id)
        {
            IQueryable<GetCategoryViewModel> bookCategories = BookCategories.AsQueryable();
            var category = bookCategories
                                   .Where((element) => element.Id == id)
                                   .FirstOrDefault();


            return category.Name;
        }

        private async Task Update()
        {
            try
            {
                // Call service method to add the book
                Success = await UpdateBookRepository.UpdateAsync(_viewModel, "api/Book", _viewModel.Id.ToString());


                if (Success)
                {
                    toastService.ShowToast(ToastLevel.Success, "Book updated successfully");

                    await Task.Delay(5000);

                    NavigationManager.NavigateTo("/books");
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