using Blazored.Toast.Services;
using BookshelfXchange.Middleware;
using BookshelfXchange.Repository;
using BookshelfXchange.Services;
using BookshelfXchange.ViewModels.POST;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace BookshelfXchange.Components.Pages.Auth
{
    public partial class SignIn
    {
        [SupplyParameterFromForm]
        private LoginViewModel Login { get; set; } = new();



        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        FirebaseAuthService AuthService { get; set; }

        [Inject]
        private ICookie CookieService { get; set; }

        private bool isProcessing = false;

        [CascadingParameter]
        public Task<AuthenticationState> AuthenticationStateTask { get; set; }

        [Inject]
        public CustomAuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        private HttpClient httpClient { get; set; }

        private async Task LoginUser()
        {
            isProcessing = true;

            try
            {
                var email = Login.Email;

                // Authenticate user
                var user = await AuthService.Login(Login.Email, Login.Password);

                if (user != null)
                {
                    // Get the user's token
                    var token = await user.GetIdTokenAsync();

                    // Set cookies
                    await CookieService.SetValue("Email", Login.Email);
                    await CookieService.SetValue("FirebaseToken", token);

                    // Add token to request headers
                    //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

                    List<string> roles = jwt.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();

                    var x = new ClaimsIdentity(jwt.Claims, "FirebaseToken");
                    ClaimsPrincipal claimsPrincipal = new(x);

                    // Notify authentication state
                    AuthenticationStateProvider.SetAuthenticationState(claimsPrincipal);
                    var authState = await AuthenticationStateTask;
                    isProcessing = false;

                    // Show success message
                    toastService.ShowToast(ToastLevel.Success, "Successfully logged in.");

                    // Reset the User object
                    Login = new LoginViewModel();

                    string url = GetQueryParm("returnUrl");

                    if (string.IsNullOrEmpty(url))
                    {
                        // Redirect to the desired page
                        NavigationManager.NavigateTo("/admin", true);
                    }
                    else
                    {
                        NavigationManager.NavigateTo($"{url}");
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
                isProcessing = false;
                // Handle Firebase authentication exceptions
                AuthResultStatus status = FirebaseAuthExceptionHandler.HandleException(ex);
                string error = FirebaseAuthExceptionHandler.GenerateExceptionMessage(status);

                toastService.ShowToast(ToastLevel.Error, error);
            }
            catch (Exception ex)
            {
                isProcessing = false;
                // Log or handle other exceptions as needed
                toastService.ShowToast(ToastLevel.Error, $"Error occurred while logging in: {ex.Message}");
            }
        }

        string GetQueryParm(string parmName)
        {
            var uriBuilder = new UriBuilder(NavigationManager.Uri);
            var q = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            var returnUrl = q[parmName] ?? "";

            // Remove the ~/
            if (returnUrl.StartsWith("~/"))
            {
                returnUrl = returnUrl.Substring(2);
            }

            return returnUrl;
        }


    }
}
