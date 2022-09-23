using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.Enums;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTProjectService : IBTProjectService
    
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTRolesService _rolesService;
        private readonly IImageService _imageService;

        public BTProjectService(ApplicationDbContext context,
                                IBTRolesService rolesService,
                                IImageService imageService)
        {
            _context = context;
            _rolesService = rolesService;
            _imageService = imageService;
        }

        public async Task AddProjectAsync(Project project)
        {
            try
            {
                await _context.AddAsync(project);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            try
            {
                BTUser? currentPM = await GetProjectManagerAsync(projectId)!;
                BTUser? selectPM = await _context.Users.FindAsync(userId);

                //remove current PM
                if(currentPM != null)
                {
                    await RemoveProjectManagerAsync(projectId);
                }

                //add new PM
                try
                {
                    await AddUserToProjectAsync(selectPM!, projectId);
                    return true;
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

        public async Task<bool> AddUserToProjectAsync(BTUser user, int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId);

                bool onProject = project.Members!.Any(m => m.Id == user.Id);

                //check if user is not on project
                if (!onProject)
                {
                    project.Members!.Add(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task ArchiveProjectAsync(int projectId)
        {
            try
            {
                Project project = await GetProjectByIdAsync(projectId);
                project.Archived = true;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Project>> GetAchivedProjectsByCompanyIdAsync(int companyId)
        {
            try
            {
                List<Project>? projects = await _context.Projects
                                                        .Where(p => p.Id == companyId && p.Archived)
                                                        .Include(p => p.Company)
                                                        .Include(p => p.ProjectPriority)
                                                        .ToListAsync();
                

                return projects;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Project>> GetAllProjectsByCompanyIdAsync(int companyId)
        {
            try
            {
                List<Project> projects = await _context.Projects
                                                       .Where(p => p.CompanyId == companyId && !p.Archived)
                                                       .Include(p => p.Company)
                                                       .Include(p => p.ProjectPriority)
                                                       .ToListAsync();
                return projects;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Project> GetProjectByIdAsync(int? projectId)
        {
            try
            {
                Project? project = await _context.Projects
                .Include(p => p.Company)
                .Include(p => p.ProjectPriority)
                .Include(p => p.Members)
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync(m => m.Id == projectId);

                return project!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                List<Project>? projects = (await _context.Users
                                                         .Include(u => u.Projects)!
                                                            .ThenInclude(p => p.Company)
                                                         .Include(u => u.Projects)!
                                                            .ThenInclude(p => p.Members)
                                                         .Include(u => u.Projects)!
                                                            .ThenInclude(p => p.Tickets)
                                                         .Include(u => u.Projects)!
                                                            .ThenInclude(t => t.Tickets)!
                                                                .ThenInclude(t => t.DeveloperUser)
                                                         .Include(u => u.Projects)!
                                                             .ThenInclude(t => t.Tickets)!
                                                                 .ThenInclude(t => t.SubmitterUser)
                                                         .Include(u => u.Projects)!
                                                             .ThenInclude(t => t.Tickets)!
                                                                 .ThenInclude(t => t.TicketPriority)
                                                         .Include(u => u.Projects)!
                                                             .ThenInclude(t => t.Tickets)!
                                                                 .ThenInclude(t => t.TicketStatus)
                                                         .Include(u => u.Projects)!
                                                             .ThenInclude(t => t.Tickets)!
                                                                 .ThenInclude(t => t.TicketType)
                                                         .FirstOrDefaultAsync(u => u.Id == userId))?.Projects!.ToList();
                return projects!;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BTUser>? GetProjectManagerAsync(int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId);

                foreach(BTUser member in project.Members!)
                {
                    if(await _rolesService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager)))
                    {
                        //Remove BTUser from Project
                        //await RemoveUserFromProjectAsync(member, projectId);
                        return member;
                    } 
                }
               return null!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId);

                if(project != null)
                {
                    //Check to see if user is a project memeber
                    return  project.Members!.Any(m=>m.Id == userId);
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId);

                foreach(BTUser member in project.Members!)
                {
                    if(await _rolesService.IsUserInRoleAsync(member, nameof(BTRoles.ProjectManager))){
                        await RemoveProjectManagerAsync(projectId);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> RemoveUserFromProjectAsync(BTUser user, int projectId)
        {
            try
            {
                Project? project = await GetProjectByIdAsync(projectId);

                bool onProject = project.Members!.Any(m=>m.Id == user.Id);

                //check if user is on project
                if (onProject)
                {
                    project.Members!.Remove(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RestoreProjectAsync(int projectId)
        {
            try
            {
                Project project = await GetProjectByIdAsync(projectId);
                project.Archived = false;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            try
            {
                if (project.ImageFormFile != null)
                {
                    project.ImageFile = await _imageService.ConvertFileToByteArrayAsync(project.ImageFormFile);
                    project.ImageFileType = project.ImageFormFile.ContentType;
                }
                project.Created = DataUtility.GetPostGresDate(project.Created);
                project.StartDate = DataUtility.GetPostGresDate(project.StartDate);
                project.EndDate = DataUtility.GetPostGresDate(project.EndDate);

                _context.Update(project);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
