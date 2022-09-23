using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTRolesService : IBTRolesService
    {
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<BTUser> _userManager;
		private readonly ApplicationDbContext _context;

		public BTRolesService(RoleManager<IdentityRole> roleManager, UserManager<BTUser> userManager, ApplicationDbContext context)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
		}

		public async Task<bool> AddUserToRoleAsync(BTUser user, string roleName)
		{
			try
			{
				bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;
				return result;
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<List<IdentityRole>> GetRolesAsync()
		{
			try
			{
				List<IdentityRole> result = new();
				result = await _context.Roles.ToListAsync();
				return result;
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<IEnumerable<string>> GetUserRolesAsync(BTUser user)
		{
			try
			{
				IEnumerable<string> result = await _userManager.GetRolesAsync(user);

				return result;
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId)
        {
			try
			{
				List<BTUser> btUsers = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
				List<BTUser> results = btUsers.Where(b=>b.CompanyId == companyId).ToList();

				return results;
			}
			catch (Exception)
			{

				throw;
			}
        }

		public async Task<bool> IsUserInRoleAsync(BTUser member, string roleName)
		{
			try
			{
				bool result = await _userManager.IsInRoleAsync(member,roleName);
				return result;
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName)
		{
			try
			{
                bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;

                return result;
            }
			catch (Exception)
			{

				throw;
			}
		}
        public async Task<bool> RemoveUserFromRolesAsync(BTUser user, IEnumerable<string> roleNames)
        {
            try
            {
                bool result = (await _userManager.RemoveFromRolesAsync(user, roleNames)).Succeeded;

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
