﻿using BookshelfXchange.Constants;

namespace BookshelfXchange.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public BaseRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _apiBaseUrl = ApiUrl.ApiBaseUrl;
        }

        public async Task<List<TEntity>> GetAllAsync(string endpoint)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<TEntity>>($"{_apiBaseUrl}/{endpoint}");
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception as needed
                throw new Exception("Error occurred while fetching data from the server.", ex);
            }
        }

        public async Task<TEntity> GetByIdAsync(int id, string endpoint)
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

        public async Task<bool> AddAsync(TEntity entity, string endpoint)
        {
            try
            {
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

        public async Task<TEntity> UpdateAsync(int id, TEntity entity)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/{typeof(TEntity).Name}/{id}", entity);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TEntity>();
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception as needed
                throw new Exception($"Error occurred while updating data for ID {id} on the server.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/{typeof(TEntity).Name}/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception as needed
                throw new Exception($"Error occurred while deleting data for ID {id} from the server.", ex);
            }
        }
    }
}
