using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Project
    {
        //primary key 
        public int Id { get; set; }
        //foregn key
        public int CompanyName { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ProjectPriorityId { get; set; }
        public IFormFile? ImageFormFile {get; set;}
        public string? ImageFileName { get; set; }
        public string? ImageFileType { get; set; }
        public bool Archived { get; set; }

        //Nav Properties
        public virtual Company? Company { get; set; }
        public virtual ProjectPriority? ProjectPriority { get; set; }
        public virtual ICollection<BTUser>? Members { get; set; }
        public virtual ICollection<Ticket>? Tickets { get; set; }
    }
}
