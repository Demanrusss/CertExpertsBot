using CertExpertsBot.Models;

namespace CertExpertsBot.Services
{
    internal class TNVEDCodeService : ITNVEDCodeService
    {
        public async Task<TNVEDCode> GetByCodeAsync<TNVEDCode>(string code)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TNVEDCode>> GetByNameAsync(string searchStr)
        {
            throw new NotImplementedException();
        }
    }
}
