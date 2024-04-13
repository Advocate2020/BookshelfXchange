using Microsoft.AspNetCore.Authentication;

namespace BookShelfXChange.Services
{
    public static class StateHandler
    {
        public static void SetUserState(HttpContext http, string token, string refreshToken)
        {
            http.Response.Cookies.Append("idToken", token);
            http.Response.Cookies.Append("refreshToken", refreshToken);
        }

        public static void DeleteState(HttpContext http, string token)
        {
            http.Session.Remove(token);
        }

        public static void RemoveUser(HttpContext http)
        {
            http.SignOutAsync();
        }

    }
}
