namespace BookShelfXChange.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(string endpoint);
        Task<T> GetByIdAsync(string id, string endpoint);
        Task<bool> AddAsync(T entity, string endpoint);
        Task<bool> AddWithFileAsync(MultipartFormDataContent T, string endpoint);
        Task<bool> UpdateAsync(T entity, string endpoint, string id);
        Task<bool> DeleteAsync(string endpoint, int id);
    }
}

