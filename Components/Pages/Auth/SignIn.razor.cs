using Blazored.Toast.Services;
using BookshelfXchange.Models;
using BookshelfXchange.Repository;
using BookshelfXchange.Services;
using BookshelfXchange.ViewModels.POST;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace BookshelfXchange.Components.Pages.Auth
{
    public partial class SignIn
    {
        [SupplyParameterFromForm]
        private LoginViewModel Login { get; set; } = new();

        [Inject]
        private IBaseRepository<LoginViewModel> LoginRepository { get; set; }

        [Inject]
        NavigationManager NavigationManager { get; set; }

        [Inject]
        FirebaseAuthService AuthService { get; set; }

        private bool isProcessing = false;

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

                    var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

                    List<string> roles = jwt.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();

                    var isGeneralUser = roles.Contains(RoleTypes.USER.Name);
                    var isAdmin = roles.Contains(RoleTypes.ADMIN.Name);

                    if (!isAdmin && !isGeneralUser)
                    {
                        isProcessing = false;
                        toastService.ShowToast(ToastLevel.Error, "You are unauthorized.");


                    }

                    var x = new ClaimsIdentity(jwt.Claims, ".MySessionCookie");
                    ClaimsPrincipal claimsPrincipal = new(x);
                    var httpContext = HttpContextAccessor.HttpContext;
                    await httpContext.SignInAsync(".MySessionCookie", claimsPrincipal);

                    StateHandler.SetUserState(httpContext, token);

                    isProcessing = false;

                    if (isAdmin)
                    {

                        // Redirect to the desired page
                        NavigationManager.NavigateTo("/admin");
                        return;
                    }
                    else if (isGeneralUser)
                    {
                        // Redirect to the desired page
                        //NavigationManager.NavigateTo("/admin");
                        // Show success message
                        toastService.ShowToast(ToastLevel.Success, "Successfully logged in.");
                    }

                }
                else
                {
                    isProcessing = false;
                    // Handle unsuccessful login
                    throw new Exception("Failed to log in. Invalid credentials.");
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

    }
}
