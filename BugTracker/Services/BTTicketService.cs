using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Evaluation;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTTicketService : IBTTicketService
    {
		private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;

        public BTTicketService(ApplicationDbContext context, UserManager<BTUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> AssignDeveloperAsync(string userId, int ticketId)
		{
                
                try
                {
                    Ticket? ticket = await GetTicketByIdAsync(ticketId);
                    
                        ticket.DeveloperUserId = userId;
                        await _context.SaveChangesAsync();
                        return true;
                    
                }
                catch (Exception)
                {

                    throw;
                }

                
           
         }
        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            List<Ticket>? tickets = await _context.Tickets
                                               .Include(t => t.DeveloperUser)
                                               .Include(t => t.Project)
                                               .Include(t => t.SubmitterUser)
                                               .Include(t => t.TicketPriority)
                                               .Include(t => t.TicketStatus)
                                               .Include(t => t.TicketType)
                                               .Include(t => t.Comments)
                                               .ThenInclude(t => t.User)
                                               .ToListAsync();

            return tickets!;
        }

        public async Task<Ticket> GetTicketByIdAsync(int? ticketId)
        {
            Ticket? ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.Project)
                .Include(t => t.SubmitterUser)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.TicketAttachments)
                .Include(t => t.Comments)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == ticketId);

            return ticket!;
        }
      
    
		public async Task<Ticket> GetTicketAsNoTrackingAsync(int ticketId, int companyId)
        {
			try
			{
				Ticket? ticket = new();

				ticket = await _context.Projects
										.Where(p => p.CompanyId == companyId && !p.Archived)
									    .SelectMany(p => p.Tickets!)
											.Include(t => t.TicketAttachments)
											.Include(t => t.Comments)
											.Include(t => t.History)
											.Include(t => t.DeveloperUser)
											.Include(t => t.SubmitterUser)
											.Include(t => t.TicketPriority)
											.Include(t => t.TicketStatus)
											.Include(t => t.TicketType)
											.Include(t => t.Project)
											.Where(t => !t.Archived && !t.ArchivedByProject)
										.AsNoTracking()
										.FirstOrDefaultAsync(t => t.Id == ticketId);

                   return ticket!;

			}
			catch (Exception)
			{

				throw;
			}
        }

        public async Task<bool> AddCommentAsync(TicketComment ticketComment)
        { 

            _context.Add(ticketComment);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Ticket>> GetArchivedTicketsAsync()
        {
            List<Ticket>? tickets = await _context.Tickets
                                               .Where(t => t.Archived == true)
                                               .Include(t => t.DeveloperUser)
                                               .Include(t => t.Project)
                                               .Include(t => t.SubmitterUser)
                                               .Include(t => t.TicketPriority)
                                               .Include(t => t.TicketStatus)
                                               .Include(t => t.TicketType)
                                               .Include(t => t.Comments)
                                               .ThenInclude(t => t.User)
                                               .ToListAsync(); 
            return tickets;
        }

        public async Task<List<Ticket>> GetUnassignedTicketsAsync()
        {
            try
            {
                List<Ticket> tickets = await _context.Tickets
                                                       .Where(t => t.DeveloperUserId == null)
                                                       .Include(t => t.DeveloperUser)
                                                       .Include(t => t.Project)
                                                       .Include(t => t.SubmitterUser)
                                                       .Include(t => t.TicketPriority)
                                                       .Include(t => t.TicketStatus)
                                                       .Include(t => t.TicketType)
                                                       .Include(t => t.Comments)
                                                        .ThenInclude(t => t.User)
                                                       .ToListAsync();
                return tickets;
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        public async Task AddTicketAttachmentAsync(TicketAttachment ticketAttachment)
        {
            try
            {
                await _context.AddAsync(ticketAttachment);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
