namespace ManageDb.Services
{
    public interface ITNVEDCodeService<T> : IService<T>
    {
        Task<ICollection<T>> GetByCodeAsync(string searchStr);
        Task<ICollection<T>> GetAllWithTechRegsAsync(int page, int pageSize);
    }
}
