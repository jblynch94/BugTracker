using Microsoft.AspNetCore.Mvc.Rendering;

namespace BugTracker.Models.ViewModels
{
    public class AssignDevViewModel
    {
        public Ticket? Ticket { get; set; }
        public SelectList? DeveloperList { get; set; }
        public string? Developer { get; set; }
    }
}
