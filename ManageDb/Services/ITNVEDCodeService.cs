namespace ManageDb.Services
{
    public interface ITNVEDCodeService<T> : IService<T>
    {
        public Task<ICollection<T>> GetByCodeAsync(string searchStr);
    }
}
