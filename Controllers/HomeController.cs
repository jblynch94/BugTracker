using Microsoft.AspNetCore.Mvc;
using BugTracker.Models;
using System.Diagnostics;
using BugTracker.Models.ViewModels;
using BugTracker.Services.Interfaces;
using BugTracker.Extensions;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBTCompanyService _companyService;
        private readonly IBTProjectService _projectService;
        private readonly IBTTicketService _ticketService;
        public HomeController(ILogger<HomeController> logger, IBTCompanyService companyService, IBTProjectService projectService, IBTTicketService ticketService)
        {
            _logger = logger;
            _companyService = companyService;
            _projectService = projectService;
            _ticketService = ticketService;
        }

        public IActionResult Index()
        {
            return View();
        }

        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(DashboardViewModel model)
        {
            int companyId = User.Identity!.GetCompanyId();
            model.Company = await _companyService.GetCompanyInfoAsync(companyId);
            model.Members = await _companyService.GetMemebersAsync(companyId);
            model.Projects = await _projectService.GetAllProjectsByCompanyIdAsync(companyId);
            model.Tickets = await _ticketService.GetAllTicketsAsync();
            return View(model);
        }
    }
}