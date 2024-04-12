namespace ManageDb.Services
{
    public interface IService<T>
    {
        Task<ICollection<T>> GetAllAsync();
        Task<ICollection<T>> GetAllAsync(int page, int pageSize);
        Task<T> GetByIdAsync(int id);
        Task<int> AddAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task<int> DeleteAsync(int id);
        Task<int> GetCountAsync();
    }
}
