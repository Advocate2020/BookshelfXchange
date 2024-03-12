namespace BookshelfXchange.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(string endpoint);
        Task<T> GetByIdAsync(int id, string endpoint);
        Task<bool> AddAsync(T entity, string endpoint);
        Task<T> UpdateAsync(int id, T entity);
        Task<bool> DeleteAsync(int id);
    }
}

