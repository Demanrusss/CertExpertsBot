namespace ManageDb.Services;

public interface ITechRegService<T> : IService<T>
{
    Task<int> UpdateTNVEDCodesAsync(int techRegId, IReadOnlyCollection<int> tnvedCodeIds);
}