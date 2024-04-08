using CertExpertsBot.Models;

namespace CertExpertsBot.Services
{
    internal interface ITNVEDCodeService
    {
        Task<TNVEDCode> GetByCodeAsync<TNVEDCode>(string code);
        Task<IEnumerable<TNVEDCode>> GetByNameAsync(string searchStr);
    }
}
