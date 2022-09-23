using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Project
    {
        //primary key 
        public int Id { get; set; }
        //foregn key
        public int CompanyId { get; set; }
        [Required]
        [DisplayAttribute(Name = "Project Name")]
        [StringLength(300, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
        public string? Name { get; set; }
        [Required]
        [DisplayAttribute(Name = "Project Description")]
        [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and a max {1} characters long.", MinimumLength = 2)]
        public string? Description { get; set; }
        [DataType(DataType.Date)]
        [DisplayAttribute(Name = "Date Created")]
        public DateTime Created { get; set; }
        [DataType(DataType.Date)]
        [DisplayAttribute(Name = "Project Start Date")]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayAttribute(Name = "Project End Date")]
        public DateTime EndDate { get; set; }
        //foregn key
        public int ProjectPriorityId { get; set; }
        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile? ImageFormFile {get; set;}
        public byte[]? ImageFile { get; set; }
        public string? ImageFileType { get; set; }
        public bool Archived { get; set; }

        //Nav Properties
        public virtual Company? Company { get; set; }
        public virtual ProjectPriority? ProjectPriority { get; set; }
        public virtual ICollection<BTUser>? Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Ticket>? Tickets { get; set; } = new HashSet<Ticket>();
    }
}
