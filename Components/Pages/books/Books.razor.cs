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
        private IBaseRepository<GetBookViewModel> GetBookRepository { get; set; }

        private PaginationState PaginationState { get; set; }

        private string titleFilter { get; set; }



        IQueryable<GetBookViewModel> FilteredBookTitle
        {
            get
            {
                var result = bookItems;

                if (!string.IsNullOrEmpty(titleFilter))
                {
                    result = result.Where((book) => book.Title.Contains(titleFilter, StringComparison.CurrentCultureIgnoreCase) || book.CategoryName.Contains(titleFilter, StringComparison.CurrentCultureIgnoreCase) || book.Author.Contains(titleFilter, StringComparison.CurrentCultureIgnoreCase));
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
                Console.WriteLine($"Error occurred while fetching books and categories: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }





    }
}