using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTTicketService
    {
        //public Task AddProjectAsync(Project project);
        //public Task ArchiveProjectAsync(int projectId);
        //public Task RestoreProjectAsync(int projectId);
        //public Task UpdateProjectAsync(Project project);

        public Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId, int companyId);

    }
}
