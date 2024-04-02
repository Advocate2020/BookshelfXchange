using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BookshelfXchange.Middleware
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NavigationManager _navigationManager;

        public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor, NavigationManager navigationManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _navigationManager = navigationManager;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var token = httpContext?.Request.Cookies["FirebaseToken"];

            if (string.IsNullOrEmpty(token))
            {
                // Token is null or empty, redirect to sign-in page

                // Return an empty authentication state
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
            }

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            List<string> roles = jwt.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var x = new ClaimsIdentity(jwt.Claims, "FirebaseToken");
            ClaimsPrincipal claimsPrincipal = new(x);

            return Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public void SetAuthenticationState(ClaimsPrincipal user)
        {
            var authenticationState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authenticationState);
        }
    }
}
