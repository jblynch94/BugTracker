using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;

namespace BugTracker.Services
{
    public class BTTicketHistoryService : IBTTicketHistoryService
    {
        private readonly ApplicationDbContext _context;

        public BTTicketHistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Add History Async (Ticket, Ticket, string)
        public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
        {
            try
            {
                //new ticket has been added
                if(oldTicket == null && newTicket != null)
                {
                    TicketHistory ticketHistory = new()
                    {
                        TicketId = newTicket.Id,
                        PropertyName = string.Empty,
                        OldValue = string.Empty,
                        NewValue = string.Empty,
                        Created = DataUtility.GetPostGresDate(DateTime.Now),
                        UserId = userId,
                        Description = "New Ticket Added."
                    };

                    try
                    {
                        await _context.AddAsync(ticketHistory);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                else
                {
                    if(oldTicket!.Title != newTicket!.Title)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Title",
                            OldValue = oldTicket.Title,
                            NewValue = newTicket.Title,
                            Created = DataUtility.GetPostGresDate(DateTime.Now),
                            UserId = userId,
                            Description = "Ticket Title has been modified."
                        };
                        await _context.AddAsync(ticketHistory);
                    }
                    if(oldTicket!.Description != newTicket!.Description)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Description",
                            OldValue = oldTicket.Description,
                            NewValue = newTicket.Description,
                            Created = DataUtility.GetPostGresDate(DateTime.Now),
                            UserId = userId,
                            Description = "Ticket Description has been modified"
                        };
                        await _context.AddAsync(ticketHistory);
                    }
                    if(oldTicket.TicketPriorityId != newTicket!.TicketPriorityId)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "TicketPriority",
                            OldValue = oldTicket.TicketPriority!.Name,
                            NewValue = newTicket.TicketPriority!.Name,
                            Created = DataUtility.GetPostGresDate(DateTime.Now),
                            UserId = userId,
                            Description = "Ticket Priority has been modified"
                        };
                        await _context.AddAsync(ticketHistory);
                    }
                    if(oldTicket.TicketStatusId != newTicket!.TicketStatusId)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "TicketStatus",
                            OldValue = oldTicket.TicketStatus!.Name,
                            NewValue = newTicket.TicketStatus!.Name,
                            Created = DataUtility.GetPostGresDate(DateTime.Now),
                            UserId = userId,
                            Description = "Ticket Status has been modified"
                        };
                        await _context.AddAsync(ticketHistory);
                    }
                    if(oldTicket.TicketTypeId != newTicket!.TicketTypeId)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "TicketType",
                            OldValue = oldTicket.TicketType!.Name,
                            NewValue = newTicket.TicketType!.Name,
                            Created = DataUtility.GetPostGresDate(DateTime.Now),
                            UserId = userId,
                            Description = "Ticket Type has been modified"
                        };
                        await _context.AddAsync(ticketHistory);
                    }
                    if(oldTicket.DeveloperUserId != newTicket!.DeveloperUserId)
                    {
                        TicketHistory ticketHistory = new()
                        {
                            TicketId = newTicket.Id,
                            PropertyName = "Developer",
                            OldValue = oldTicket.DeveloperUser!.FullName ?? "Not Assigned",
                            NewValue = newTicket.DeveloperUser!.FullName,
                            Created = DataUtility.GetPostGresDate(DateTime.Now),
                            UserId = userId,
                            Description = "Ticket Developer has been modified"
                        };
                        await _context.AddAsync(ticketHistory);
                    }
                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Add History Async (int, string, string)
        public async Task AddHistoryAsync(int ticketId, string model, string userId)
        {
            try
            {
                Ticket? ticket = await _context.Tickets.FindAsync(ticketId);

                string description = model.ToLower().Replace("ticket", "");
                description = $"New {description} added to ticket: {ticket!.Title}";

                TicketHistory ticketHistory = new()
                {
                    TicketId = ticket.Id,
                    PropertyName = model,
                    OldValue = string.Empty,
                    NewValue = string.Empty,
                    Created = DataUtility.GetPostGresDate(DateTime.Now),
                    UserId = userId,
                    Description = description
                };

                try
                {
                    await _context.AddAsync(ticketHistory);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Get Company Tickets Histories Async
        public Task<List<TicketHistory>> GetCompanyTicketsHistoryAsync(int companyId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Get Projcets Tickets Histories Async
        public Task<List<TicketHistory>> GetProjectTicketsHistoryAsync(int projectId, int companyId)
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
