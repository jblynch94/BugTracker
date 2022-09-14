using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTCompanyService
    {
        public Task<List<BTUser>> GetMemebersAsync(int? companyId);
        public Task<Company> GetCompanyInfoAsync(int? companyId);

    }
}
