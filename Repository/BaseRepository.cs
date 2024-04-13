using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BookShelfXChange.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseRepository(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _apiBaseUrl = Constants.Constants.ApiBaseUrl;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<TEntity>> GetAllAsync(string endpoint)
        {
            try
            {
                // Retrieve the current HTTP context
                var httpContext = _httpContextAccessor.HttpContext;
                var token = GetFirebaseTokenFromCookies();

                // Add the Firebase token to the request headers
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                return await _httpClient.GetFromJsonAsync<List<TEntity>>($"{_apiBaseUrl}/{endpoint}");
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception as needed
                throw new Exception("Error occurred while fetching data from the server.", ex);
            }
        }

        public async Task<TEntity> GetByIdAsync(string id, string endpoint)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<TEntity>($"{_apiBaseUrl}/{endpoint}/{id}");
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception as needed
                throw new Exception($"Error occurred while fetching data for ID {id} from the server.", ex);
            }
        }

        public string? GetFirebaseTokenFromCookies()
        {
            // Retrieve the current HTTP context
            var httpContext = _httpContextAccessor.HttpContext;

            // Check if the HTTP context is not null and contains cookies
            if (httpContext != null && httpContext.Request.Cookies.TryGetValue("IdToken", out var firebaseToken))
            {
                // Retrieve the Firebase token from cookies
                return firebaseToken;
            }

            // Return null if the Firebase token is not found in cookies
            return null;
        }

        public async Task<bool> AddAsync(TEntity entity, string endpoint)
        {
            try
            {
                // Retrieve the current HTTP context
                var httpContext = _httpContextAccessor.HttpContext;
                var token = GetFirebaseTokenFromCookies();

                // Add the Firebase token to the request headers
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/{endpoint}", entity);

                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception as needed
                throw new Exception("Error occurred while adding data to the server.", ex);
            }
        }
        public async Task<bool> AddWithFileAsync(MultipartFormDataContent entity, string endpoint)
        {
            try
            {
                // Retrieve the current HTTP context
                var httpContext = _httpContextAccessor.HttpContext;
                var token = GetFirebaseTokenFromCookies();
                //_httpClient.Timeout = TimeSpan.FromMinutes(3);
                // Add the Firebase token to the request headers
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/{endpoint}", entity);

                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception as needed
                throw new Exception("Error occurred while adding data to the server.", ex);
            }
        }

        public async Task<bool> UpdateAsync(TEntity entity, string endpoint, string id)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{endpoint}/{int.Parse(id)}", entity);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception as needed
                throw new Exception($"Error occurred while updating data for ID {id} on the server.", ex);
            }
        }

        public async Task<bool> DeleteAsync(string endpoint, int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/{endpoint}/{id}");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception as needed
                throw new Exception($"Error occurred while deleting data for ID {id} from the server.", ex);
            }
        }

        private static HttpContent CreateJsonContent(object requestData)
        {
            string jsonRequestData = JsonConvert
                .SerializeObject(requestData);

            return new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
        }

    }
}
