using Blazored.Toast.Services;
using BookshelfXchange.ViewModels.GET;
using BookshelfXchange.ViewModels.POST;
using BookShelfXChange.Repository;
using BookShelfXChange.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;

namespace BookShelfXChange.Components.Pages.Admin_Pages.books
{
    public partial class CreateBook
    {
        private long maxFileSize = 1024 * 1024 * 8; // represents 3MB
        private int maxAllowedFiles = 3;
        private List<string> errors = [];
        private List<IBrowserFile> selectedFiles = new List<IBrowserFile>();
        private string extensionname = "default";
        private List<string> base64data = [];
        private string isdisplayimage;
        private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";
        private string _dragClass = DefaultDragClass;
        private readonly List<string> _fileNames = new();


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



        private async Task HandleFileSelection(InputFileChangeEventArgs e)
        {

            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                var validationResult = await FileValidator.ValidateFileAsync(file);
                if (validationResult != null)
                {
                    toastService.ShowToast(ToastLevel.Error, $"File: {file.Name} Error: {validationResult}");
                }
                else
                {
                    //get the upload file extension.
                    try
                    {
                        extensionname = Path.GetExtension(file.Name);
                        //resize the image and create the thumbnails
                        var resizedFile = await file.RequestImageFileAsync(file.ContentType, 640, 480); // resize the image file
                        var buf = new byte[resizedFile.Size]; // allocate a buffer to fill with the file's data
                        await resizedFile.OpenReadStream().ReadAsync(buf);

                        // copy the stream to the buffer

                        if (extensionname != null)
                        {
                            extensionname = extensionname.ToLower();
                            if (extensionname == ".png")
                            {
                                base64data.Add("data:image/png;base64," + Convert.ToBase64String(buf)); // convert to a base64 string for PNG
                            }
                            if (extensionname == ".jpeg")
                            {
                                base64data.Add("data:image/jpeg;base64," + Convert.ToBase64String(buf)); // convert to a base64 string for PNG
                            }
                            else if (extensionname == ".jpg")
                            {
                                base64data.Add("data:image/jpg;base64," + Convert.ToBase64String(buf)); // convert to a base64 string for JPEG
                            }
                            // Add other image types handling if necessary
                        }
                        _fileNames.Add(file.Name);

                        //then you can send the base64 data to the server side and insert it into database.




                    }
                    catch (Exception ex)
                    {

                        toastService.ShowToast(ToastLevel.Error, $"File: {file.Name} Error: {ex.Message}");
                    }



                }
                selectedFiles.Add(file);
            }


            BookModel.BookImages = selectedFiles;
        }

        private async Task Clear()
        {
            _fileNames.Clear();
            selectedFiles.Clear();
            base64data.Clear();
            BookModel.BookImages.Clear();
            ClearDragClass();
            await Task.Delay(100);
        }

        private void SetDragClass()
            => _dragClass = $"{DefaultDragClass} mud-border-primary";

        private void ClearDragClass()
            => _dragClass = DefaultDragClass;


        private async Task Create()
        {
            try
            {
                isProcessing = true;
                var requestData = GenerateRequestData(BookModel.Title, BookModel.Author, BookModel.CategoryId, BookModel.BookImages);

                // Call service method to add the book
                Success = await BookRepository.AddWithFileAsync(requestData, "api/Book");


                if (Success)
                {
                    await Task.Delay(5000);
                    isProcessing = false;

                    toastService.ShowToast(ToastLevel.Success, "Book Added successfully");

                    selectedFiles.Clear();
                    base64data.Clear();
                    BookModel = new AddBookViewModel();
                    _fileNames.Clear();


                    //await Task.Delay(5000);

                    //NavigationManager.NavigateTo("/books");
                }


            }
            catch (Exception ex)
            {
                isProcessing = false;
                // Log or handle the exception as needed
                toastService.ShowToast(ToastLevel.Error, $"Error occurred while adding the book: {ex.Message}");

                // Optionally, show an error message to the user
            }

        }

        private static MultipartFormDataContent GenerateRequestData(string title, string author, int categoryId, List<IBrowserFile> files)
        {
            var formData = new MultipartFormDataContent
            {
                { new StringContent(title), "title" },
                { new StringContent(author), "author" },
                { new StringContent(categoryId.ToString()), "categoryId" }
            };

            foreach (var file in files)
            {

                var fileContent = new StreamContent(file.OpenReadStream(file.Size));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                formData.Add(fileContent, "bookImages", file.Name);
            }

            return formData;
        }

    }
}