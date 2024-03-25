using Microsoft.AspNetCore.Authentication;

namespace BookshelfXchange.Services
{
    public static class StateHandler
    {
        public static void SetUserState(HttpContext http, string token)
        {
            http.Response.Cookies.Append("idToken", token);

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
