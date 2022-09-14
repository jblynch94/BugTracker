using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTTicketService : IBTTicketService
    {
		private readonly ApplicationDbContext _context;

		public BTTicketService(ApplicationDbContext context)
		{
			_context = context;
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
    }
}
