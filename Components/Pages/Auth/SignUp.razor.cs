using Blazored.Toast.Services;
using BookshelfXchange.Repository;
using BookshelfXchange.Services;
using BookshelfXchange.ViewModels.POST;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BookshelfXchange.Components.Pages.Auth
{
    public partial class SignUp
    {
        [SupplyParameterFromForm]
        private RegistrationViewModel User { get; set; } = new();

        [Inject]
        private IBaseRepository<RegistrationViewModel> UserRepository { get; set; }

        public string LoginMesssage { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        FirebaseAuthService AuthService { get; set; }

        [Inject]
        private ICookie CookieService { get; set; }

        private bool isProcessing = false;

        private async Task RegisterUser()
        {
            isProcessing = true;

            try
            {
                // Create a new user
                var user = await AuthService.SignUp(User.Email, User.Password);

                if (user != null)
                {
                    // Get the user's token
                    var token = await user.GetIdTokenAsync();

                    // Create an HttpClient instance to make the API call
                    using var httpClient = new HttpClient();

                    // Add the token to the request headers
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    // Serialize the user entity to JSON
                    var json = JsonSerializer.Serialize(User);

                    // Create a StringContent object with the JSON data
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    string endpoint = "api/Auth";
                    // Make the API call
                    var response = await httpClient.PostAsync($"{Constants.Constants.ApiBaseUrl}/{endpoint}", content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Set cookies
                        await CookieService.SetValue("Email", User.Email);
                        await CookieService.SetValue("FirebaseToken", token);

                        // Store user data in localStorage
                        await JS.InvokeVoidAsync("localStorage.setItem", "email", User.Email);
                        await JS.InvokeVoidAsync("localStorage.setItem", "firebaseToken", token);
                        // Show success message
                        toastService.ShowToast(ToastLevel.Success, "Successfully registered.");

                        // Reset the User object
                        User = new RegistrationViewModel();
                        isProcessing = false;


                        // Redirect to the desired page
                        NavigationManager.NavigateTo("/");
                    }
                    else
                    {
                        isProcessing = false;
                        // Handle unsuccessful response
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        throw new Exception($"Failed to register user: {errorMessage}");
                    }
                }
                else
                {
                    isProcessing = false;
                    throw new Exception("Failed to create user.");
                }
            }
            catch (FirebaseAuthException ex)
            {
                // Handle Firebase authentication exceptions
                AuthResultStatus status = FirebaseAuthExceptionHandler.HandleException(ex);
                string error = FirebaseAuthExceptionHandler.GenerateExceptionMessage(status);

                toastService.ShowToast(ToastLevel.Error, error);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions as needed
                toastService.ShowToast(ToastLevel.Error, $"Error occurred while adding the user: {ex.Message}");
            }
        }


    }
}