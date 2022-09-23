using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTTicketService
    { 

        public Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId, int companyId);
        public Task<List<Ticket>> GetAllTicketsAsync();
        public Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId);
        public Task<List<Ticket>> GetArchivedTicketsAsync();
        public Task<List<Ticket>> GetUnassignedTicketsAsync();  
        public Task<bool> AssignDeveloperAsync(string userId, int ticketId);
        public Task<Ticket> GetTicketByIdAsync(int? ticketId);
        public Task<bool> AddCommentAsync(TicketComment ticketComment);
        public Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment);
        public Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId);
    }
}
