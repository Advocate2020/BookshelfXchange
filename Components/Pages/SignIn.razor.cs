using Blazored.Toast.Services;
using BookshelfXchange.ViewModels.POST;
using BookShelfXChange.Models;
using BookShelfXChange.Services;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookShelfXChange.Components.Pages
{
    [AllowAnonymous]
    public partial class SignIn
    {
        [SupplyParameterFromForm]
        private LoginViewModel Login { get; set; } = new();

        [Inject]
        FirebaseAuthService AuthService { get; set; }

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = default!;

        private bool isProcessing = false;
        private bool isAdmin = false;
        private bool isUser = false;

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (HttpMethods.IsGet(HttpContext.Request.Method))
            {
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync("devAuth");
            }
        }


        private async Task LoginUser()
        {
            isProcessing = true;

            try
            {
                var email = Login.Email;

                // Authenticate user
                var tokens = await AuthService.Login(Login.Email, Login.Password);

                if (tokens.IdToken != null)
                {
                    var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokens.IdToken);

                    List<string> roles = jwt.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();

                    var x = new ClaimsIdentity(jwt.Claims, "FirebaseToken");
                    x.AddClaim(new Claim(ClaimTypes.Name, Login.Email));
                    ClaimsPrincipal claimsPrincipal = new(x);


                    await HttpContext.SignInAsync("devAuth", claimsPrincipal);

                    StateHandler.SetUserState(HttpContext, tokens.IdToken, tokens.RefreshToken);


                    toastService.ShowToast(ToastLevel.Success, "Successfully logged in.");
                    // Reset the User object
                    Login = new LoginViewModel();
                    isProcessing = false;



                    isAdmin = roles.Contains(RoleTypes.ADMIN.Name);
                    isUser = roles.Contains(RoleTypes.USER.Name);


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

                //toastService.ShowToast(ToastLevel.Error, error);
            }
            catch (Exception ex)
            {
                isProcessing = false;
                // Log or handle other exceptions as needed
                //toastService.ShowToast(ToastLevel.Error, $"Error occurred while logging in: {ex.Message}");
            }
            finally
            {
                string url = GetQueryParm("returnUrl");

                if (isAdmin && string.IsNullOrEmpty(ReturnUrl))
                {
                    // Show success message

                    RedirectManager.NavigateTo("/admin");
                }
                else if (isUser)
                {

                    RedirectManager.NavigateTo(url);

                }
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
