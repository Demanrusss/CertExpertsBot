namespace ManageDb.Services
{
    public interface ITechRegService<T> : IService<T>
    {
        Task<ICollection<T>> GetByNameAsync(string searchStr);
    }
}
