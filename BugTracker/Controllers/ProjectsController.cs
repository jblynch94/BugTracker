using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BugTracker.Services.Interfaces;
using BugTracker.Models.ViewModels;
using BugTracker.Models.Enums;
using BugTracker.Services;
using System.ComponentModel.Design;
using BugTracker.Extensions;

namespace BugTracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IBTProjectService _btProjectService;
        private readonly IBTRolesService _btRolesService;
        private readonly IBTCompanyService _btCompanyService;

        public ProjectsController(ApplicationDbContext context, UserManager<BTUser> userManager, IImageService imageService, IBTProjectService btProjectService, IBTRolesService btRolesService, IBTCompanyService btCompanyService)
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
            _btProjectService = btProjectService;
            _btRolesService = btRolesService;
            _btCompanyService = btCompanyService;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            int companyId = User.Identity!.GetCompanyId();

            List<Project> projects = await _btProjectService.GetAllProjectsByCompanyIdAsync(companyId);

            return View(projects);
        }

        //Get: AssignUsers
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignMembers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AssignUsersViewModel model = new();

            //Get CompanyId
            int companyId = User.Identity!.GetCompanyId();

            model.Project = await _btProjectService.GetProjectByIdAsync(id.Value);

            //Get current Members (if exists)
            List<string> members = model.Project.Members!.Select(m => m.Id).ToList();

            //Service call to RoleService
            model.MemberList = new MultiSelectList(await _btCompanyService.GetMemebersAsync(companyId), "Id", "FullName", members);

            return View(model);
        }

        //Post: AssignUsers
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMembers(AssignUsersViewModel model)
        {
            //Get current Members (if exists)
            List<string> members = model.Project!.Members!.Select(m => m.Id).ToList();

            if (model.Members != null)
            {
      
                foreach(string memberId in model.Members)
                {
                    BTUser? member = _context.Users.FirstOrDefault(u => u.Id == memberId);
                    await _btProjectService.AddUserToProjectAsync(member!, model.Project!.Id);

                }

                return RedirectToAction(nameof(Index));
            }
            //return RedirectToAction(nameof(AssignProjectManager), new { id = model.Project!.Id });

            ModelState.AddModelError("Members", "Please assign some Members");

            //Get CompanyId
            int companyId = User.Identity!.GetCompanyId();

            model.Project = await _btProjectService.GetProjectByIdAsync(model.Project!.Id);

            //Service call to RoleService
            model.MemberList = new MultiSelectList(await _btCompanyService.GetMemebersAsync(companyId), "Id", "FullName", members);

            return View(model);
        }

        //Get: RemoveUsers
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> RemoveMembers(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AssignUsersViewModel model = new();

            //Get CompanyId
            int companyId = User.Identity!.GetCompanyId();

            model.Project = await _btProjectService.GetProjectByIdAsync(id.Value);

            //Get current Members (if exists)
            List<string> members = model.Project.Members!.Select(m => m.Id).ToList();

            //Service call to RoleService
            model.MemberList = new MultiSelectList(await _btCompanyService.GetMemebersAsync(companyId), "Id", "FullName", members);

            return View(model);
        }

        //Post: RemoveUsers
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMembers(AssignUsersViewModel model)
        {
            //Get current Members (if exists)
            List<string> members = model.Project!.Members!.Select(m => m.Id).ToList();

            if (model.Members != null)
            {

                foreach (string memberId in model.Members)
                {
                    BTUser? member = _context.Users.FirstOrDefault(u => u.Id == memberId);
                    await _btProjectService.RemoveUserFromProjectAsync(member!, model.Project!.Id);

                }

                return RedirectToAction(nameof(Index));
            }
            //return RedirectToAction(nameof(AssignProjectManager), new { id = model.Project!.Id });

            ModelState.AddModelError("Members", "Please assign some Members");

            //Get CompanyId
            int companyId = User.Identity!.GetCompanyId();

            model.Project = await _btProjectService.GetProjectByIdAsync(model.Project!.Id);

            //Service call to RoleService
            model.MemberList = new MultiSelectList(await _btCompanyService.GetMemebersAsync(companyId), "Id", "FullName", members);

            return View(model);
        }

        //Get: AssignProjectManager
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignProjectManager(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            AssignPMViewModel model = new();

            //Get CompanyId
            int companyId = User.Identity!.GetCompanyId();

            model.Project = await _btProjectService.GetProjectByIdAsync(id.Value);

            //Get current PM (if exists)
            string? currentPMId = (await _btProjectService.GetProjectManagerAsync(model.Project.Id)!)?.Id;

            //Service call to RoleService
            model.PMList = new SelectList(await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId), "Id","FullName", currentPMId);

            return View(model);
        }

        //Post: AssignProjectManager
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignProjectManager(AssignPMViewModel model)
        {
            if (!string.IsNullOrEmpty(model.PMID))
            {
                await _btProjectService.AddProjectManagerAsync(model.PMID, model.Project!.Id);

                return RedirectToAction(nameof(Index));
            }
            //return RedirectToAction(nameof(AssignProjectManager), new { id = model.Project!.Id });

            ModelState.AddModelError("PMID","Please assign a PM");

            //Get CompanyId
            int companyId = (await _userManager.GetUserAsync(User)).CompanyId;

            model.Project = await _btProjectService.GetProjectByIdAsync(model.Project!.Id); 

            //Get current PM (if exists)
            string? currentPMId = (await _btProjectService.GetProjectManagerAsync(model.Project.Id)!)?.Id;

            //Service call to RoleService
            model.PMList = new SelectList(await _btRolesService.GetUsersInRoleAsync(nameof(BTRoles.ProjectManager), companyId), "Id", "FullName");

            return View(model);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _btProjectService.GetProjectByIdAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public IActionResult Create()
        {
            //TODO: abstract the use of _context
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name");
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id");
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyId,Name,Description,Created,StartDate,EndDate,ProjectPriorityId,ImageFile,ImageFileType,ImageFormFile,Archived")] Project project)
        {
            if (ModelState.IsValid)
            {
                //todo:Make companyId retrival more efficient
                //companyId
                project.CompanyId = (await _userManager.GetUserAsync(User)).CompanyId;

                //Date(s)
                project.Created = DataUtility.GetPostGresDate(DateTime.Now);
                project.StartDate = DataUtility.GetPostGresDate(project.StartDate);
                project.EndDate = DataUtility.GetPostGresDate(project.EndDate);

                //image
                if (project.ImageFormFile != null)
                {
                    project.ImageFile = await _imageService.ConvertFileToByteArrayAsync(project.ImageFormFile);
                    project.ImageFileType = project.ImageFormFile.ContentType;
                }

                if (!User.IsInRole(nameof(BTRoles.DemoUser)))
                {
                    await _btProjectService.AddProjectAsync(project);

                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id", project.ProjectPriorityId);
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
               .Include(p => p.Company)
               .Include(p => p.ProjectPriority)
               .FirstOrDefaultAsync(m => m.Id == id);


            if (project == null)
            {
                return NotFound();
            }

            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id", project.ProjectPriorityId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyId,Name,Description,Created,StartDate,EndDate,ProjectPriorityId,ImageFile,ImageFileType,Archived")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _btProjectService.UpdateProjectAsync(project);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompanyId"] = new SelectList(_context.Companies, "Id", "Name", project.CompanyId);
            ViewData["ProjectPriorityId"] = new SelectList(_context.ProjectPriorities, "Id", "Id", project.ProjectPriorityId);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Company)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Projects'  is null.");
            }
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> AllProjects()
        {
            //TODO: create service to get blogposts
            var applicationDbContext = _context.Projects.Include(p => p.Company).Include(p => p.ProjectPriority);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> MyProjects()
        {

            var applicationDbContext = _context.Projects.Include(p => p.Company).Include(p => p.ProjectPriority).Include(p => p.Members);
            
            return View(await applicationDbContext.ToListAsync());

        }
        public async Task<IActionResult> ArchivedProjects()
        {

            var applicationDbContext = _context.Projects.Where(p => p.Archived == true).Include(p => p.Company).Include(p => p.ProjectPriority);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> UnassignedProjects()
        {
            var companyId = User.Identity!.GetCompanyId();
            List<Project> projects = await _btProjectService.GetAllProjectsByCompanyIdAsync(companyId);
            
            
               
            
            
            return View(projects);
        }
    }
}
