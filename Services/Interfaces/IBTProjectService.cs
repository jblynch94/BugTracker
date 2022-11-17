using BugTracker.Models;

namespace BugTracker.Services.Interfaces
{
    public interface IBTProjectService
    {
        public Task<bool> AddProjectManagerAsync(string userId, int projectId);
        public Task AddProjectAsync(Project project);
        public Task<bool> AddUserToProjectAsync(BTUser user, int projectId);
        public Task ArchiveProjectAsync(int projectId);

        ///// <summary>
        ///// Gets all non archived projects by company Id
        ///// </summary>
        ///// <param name="companyId"></param>
        ///// <returns></returns>
        public Task<List<Project>> GetAllProjectsByCompanyIdAsync(int companyId);
        public Task<List<Project>> GetAchivedProjectsByCompanyIdAsync(int companyId);
        public Task<List<BTUser>> GetProjectMembersByRoleAsync(int projectId, string role);
        public Task<Project> GetProjectByIdAsync(int? projectId);
        public Task<List<Project>> GetUserProjectsAsync(string userId);
        public Task<BTUser>? GetProjectManagerAsync(int projectId);
        public Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priority);
        public Task<bool> IsUserOnProjectAsync(string userId, int projectId);  
        public Task RemoveProjectManagerAsync(int projectId);
        public Task<bool> RemoveUserFromProjectAsync(BTUser user, int projectId);
        public Task RestoreProjectAsync(int projectId);
        public Task UpdateProjectAsync(Project project);
    }
}
