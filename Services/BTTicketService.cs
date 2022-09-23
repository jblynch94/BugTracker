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
        private readonly IBTRolesService _rolesService;
        private readonly IBTProjectService _projectService;

        public BTTicketService(ApplicationDbContext context, UserManager<BTUser> userManager, IBTRolesService rolesService, IBTProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _rolesService = rolesService;
            _projectService = projectService;
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
                                               .Include(t => t.Comments)!
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
                .Include(t => t.History)
                .Include(t => t.TicketAttachments)
                .Include(t => t.Comments)!
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == ticketId);

            return ticket!;
        }

        public async Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            BTUser? btUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            List<Ticket>? tickets = new();
            try
            {
                if (await _rolesService.IsUserInRoleAsync(btUser!, nameof(BTRoles.Admin)))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyIdAsync(companyId))
                                                    .SelectMany(p => p.Tickets!).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser!, nameof(BTRoles.Developer)))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyIdAsync(companyId))
                                                    .SelectMany(p => p.Tickets!)
                                                    .Where(t => t.DeveloperUserId == userId || t.SubmitterUserId == userId).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser!, nameof(BTRoles.Submitter)))
                {
                    tickets = (await _projectService.GetAllProjectsByCompanyIdAsync(companyId))
                                                    .SelectMany(t => t.Tickets!).Where(t => t.SubmitterUserId == userId).ToList();
                }
                else if (await _rolesService.IsUserInRoleAsync(btUser!, nameof(BTRoles.ProjectManager)))
                {
                    List<Ticket>? projectTickets = (await _projectService.GetUserProjectsAsync(userId)).SelectMany(t => t.Tickets!).ToList();
                    List<Ticket>? submittedTickets = (await _projectService.GetAllProjectsByCompanyIdAsync(companyId))
                                                    .SelectMany(p => p.Tickets!).Where(t => t.SubmitterUserId == userId).ToList();
                    tickets = projectTickets.Concat(submittedTickets).ToList();
                }
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
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
                                               .Include(t => t.Comments)!
                                                 .ThenInclude(c => c.User)
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
                                                       .Include(t => t.Comments)!
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

        public async Task<TicketAttachment> GetTicketAttachmentByIdAsync(int ticketAttachmentId)
        {
            try
            {
                TicketAttachment? ticketAttachment = await _context.TicketAttachment
                                                                  .Include(t => t.User)
                                                                  .FirstOrDefaultAsync(t => t.Id == ticketAttachmentId);
                return ticketAttachment!;
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
