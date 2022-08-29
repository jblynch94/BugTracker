using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFormFile { get; set; }
        public string? ImageFileName { get; set; }
        public string? ImageFileType { get; set; }

        //Nav Properties
        public virtual ICollection<Project>? Projects { get; set; }
        public virtual ICollection<BTUser>? Members { get; set; }
        public virtual ICollection<Invite>? Invites { get; set; }


    }
}
