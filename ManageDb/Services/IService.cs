namespace ManageDb.Services
{
    public interface IService<T>
    {
        public Task<ICollection<T>> GetAllAsync();
        public Task<T> GetByIdAsync(int id);
        public Task<int> AddAsync(T entity);
        public Task<int> UpdateAsync(T entity);
        public Task<int> DeleteAsync(int id);
    }
}
