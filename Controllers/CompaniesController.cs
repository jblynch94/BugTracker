using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Models.ViewModels;
using BugTracker.Extensions;
using BugTracker.Services.Interfaces;

namespace BugTracker.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTCompanyService _companyService;
        private readonly IBTRolesService _rolesService;

        public CompaniesController(ApplicationDbContext context, IBTCompanyService bTCompanyService, IBTRolesService rolesService)
        {
            _context = context;
            _companyService = bTCompanyService;
            _rolesService = rolesService;
        }

        //// GET: Companies
        //public async Task<IActionResult> Index()
        //{
        //      return _context.Companies != null ? 
        //                  View(await _context.Companies.ToListAsync()) :
        //                  Problem("Entity set 'ApplicationDbContext.Companies' is null.");
        //}

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            int companyId = User.Identity!.GetCompanyId();
            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == companyId);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        public async Task<IActionResult> ManageUserRoles()
        {
            //1-add an instance of the veiwmodel as a alist (model)
            List<ManageUserRolesViewModel> model = new();

            //2-get companyId
            int companyId = User.Identity!.GetCompanyId();
            //3- get all company users
            List<BTUser> members = await _companyService.GetMemebersAsync(companyId);
            //4- loop over the users to populate the viewmodel
            //-instantiate single viewmodel
            //-use _rolesService
            //-create multiselects
            foreach(BTUser member in members)
            {
                ManageUserRolesViewModel viewModel = new();
                IEnumerable<string> currentRoles = await _rolesService.GetUserRolesAsync(member);

                viewModel.BTUser = member;
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(),"Name","Name", currentRoles);

                model.Add(viewModel);

            }
            //5-retrun the model to the View


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            //1- get the company Id
            int companyId = User.Identity!.GetCompanyId();
            //2- Instantiate the BTUser
            BTUser? user = (await _companyService.GetMemebersAsync(companyId)).FirstOrDefault(m => m.Id == member.BTUser!.Id);
            //3- Get Roles for the User
            IEnumerable<string> currentRoles = await _rolesService.GetUserRolesAsync(user!);
            //4- get selected role(s) for the user
            string? selectedRole = member.SelectedRoles!.FirstOrDefault();
            //5- Remove current role(s) and add new role(s)
            if(!string.IsNullOrEmpty(selectedRole))
            {
                if(await _rolesService.RemoveUserFromRolesAsync(user!, currentRoles))
                {
                    await _rolesService.AddUserToRoleAsync(user!, selectedRole);
                }
            }
            //6- Navigate
            return RedirectToAction(nameof(ManageUserRoles));
        }

        // GET: Companies/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageFile,ImageFileType")] Company company)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(company);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(company);
        //}

        // GET: Companies/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.Companies == null)
        //    {
        //        return NotFound();
        //    }

        //    var company = await _context.Companies.FindAsync(id);
        //    if (company == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(company);
        //}

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageFile,ImageFileType")] Company company)
        //{
        //    if (id != company.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(company);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CompanyExists(company.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(company);
        //}

        //// GET: Companies/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Companies == null)
        //    {
        //        return NotFound();
        //    }

        //    var company = await _context.Companies
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (company == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(company);
        //}

        //// POST: Companies/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Companies == null)
        //    {
        //        return Problem("Entity set 'ApplicationDbContext.Companies'  is null.");
        //    }
        //    var company = await _context.Companies.FindAsync(id);
        //    if (company != null)
        //    {
        //        _context.Companies.Remove(company);
        //    }
            
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CompanyExists(int id)
        //{
        //  return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
    }
}
