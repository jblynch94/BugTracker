using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services;
using BugTracker.Extensions;
using System.ComponentModel.Design;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BugTracker.Controllers
{
    public class InvitesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;
        private readonly IDataProtector _protector;
        private readonly IBTCompanyService _companyService;
        private readonly IEmailSender _emailService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTInviteService _inviteService;

        public InvitesController(ApplicationDbContext context, IBTProjectService projectService, IDataProtectionProvider dataProtectionProvider, IBTCompanyService companyService, IEmailSender emailService, UserManager<BTUser> userManager, IBTInviteService inviteService)
        {
            _context = context;
            _projectService = projectService;
            _protector = dataProtectionProvider.CreateProtector("JBL.BugTr@cker.2022");
            _companyService = companyService;
            _emailService = emailService;
            _userManager = userManager;
            _inviteService = inviteService;
        }

        // GET: Invites
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Invite.Include(i => i.Company).Include(i => i.Invitee).Include(i => i.Invitor).Include(i => i.Project);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Invites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Invite == null)
            {
                return NotFound();
            }

            var invite = await _context.Invite
                .Include(i => i.Company)
                .Include(i => i.Invitee)
                .Include(i => i.Invitor)
                .Include(i => i.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invite == null)
            {
                return NotFound();
            }

            return View(invite);
        }

        // GET: Invites/Create
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity!.GetCompanyId();

            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyIdAsync(companyId), "Id", "Name");
            return View();
        }

        // POST: Invites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,InviteeEmail,InviteeFirstName,InviteeLastName,Message")] Invite invite)
        { 
            ModelState.Remove("InvitorId");
            
            int companyId = User.Identity!.GetCompanyId();

            if (ModelState.IsValid)
            {
                try
                {
                    //encrypt code for invite
                    Guid guid = Guid.NewGuid();

                    string token = _protector.Protect(guid.ToString());
                    string email = _protector.Protect(invite.InviteeEmail!);
                    string company = _protector.Protect(companyId.ToString());

                    string? callbackUrl = Url.Action("ProcessInvite", "Invites", new { token, email, company }, protocol:Request.Scheme);

                    string body = $@"{invite.Message} <br />
                              Please join my Company. <br />
                              Click the following link to join our team. <br />
                              <a href=""{callbackUrl}"">COLLABORATE</a>";

                    string? destination = invite.InviteeEmail;

                    Company btCompany = await _companyService.GetCompanyInfoAsync(companyId);

                    string? subject = $"Bug Tracker: {btCompany.Name} Invite";

                    await _emailService.SendEmailAsync(destination, subject, body);

                    //save invite in the DB
                    invite.CompanyToken = guid;
                    invite.CompanyId = companyId;
                    invite.InviteDate = DataUtility.GetPostGresDate(DateTime.Now);
                    invite.InvitorId = _userManager.GetUserId(User);
                    invite.IsValid = true;

                    //Add Invite Service method for "AddNewInviteAsync"
                    await _inviteService.AddNewInviteAsync(invite);

                    return RedirectToAction("Index", "Home");

                    //TODO: possibly user SWAL message

                }
                catch (Exception)
                {

                    throw;
                }

                
            }

            

            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyIdAsync(companyId), "Id", "Description");
            return View(invite);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ProcessInvite(string token, string email, string company)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(company))
            {
                return NotFound();
            }

            Guid companyToken = Guid.Parse(_protector.Unprotect(token));
            string? inviteeEmail = _protector.Unprotect(email);
            int companyId = int.Parse(_protector.Unprotect(company));

            try
            {
                Invite? invite = await _inviteService.GetInviteAsync(companyToken, inviteeEmail, companyId);

                if(invite != null)
                {
                    return View(invite);
                }

                return NotFound();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
