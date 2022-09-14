﻿using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Services
{
    public class BTCompanyService : IBTCompanyService
    {
        private readonly ApplicationDbContext _context;

        public BTCompanyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company> GetCompanyInfoAsync(int? companyId)
        {
            try
            {
                Company? company = new();

                if(companyId != null)
                {
                    company = await _context.Companies
                                            .Include(c => c.Projects)
                                            .Include(c => c.Members)
                                            .Include(c => c.Invites)
                                            .FirstOrDefaultAsync(c => c.Id == companyId);
                }
                return company!;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BTUser>> GetMemebersAsync(int? companyId)
        {
            try
            {
                List<BTUser>? members = new List<BTUser>();

                    members = await _context.Users
                                            .Where(u => u.CompanyId == companyId)
                                            .ToListAsync();
                
                return members!;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
