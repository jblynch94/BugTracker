﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Company Name")]
        public string? Name { get; set; }
        [DisplayName("Company Description")]
        public string? Description { get; set; }
        [NotMapped]
        public IFormFile? ImageFormFile { get; set; }
        public byte[]? ImageFile { get; set; }
        public string? ImageFileType { get; set; }

        //Nav Properties
        public virtual ICollection<Project>? Projects { get; set; } = new HashSet<Project>();
        public virtual ICollection<BTUser>? Members { get; set; } = new HashSet<BTUser>();
        public virtual ICollection<Invite>? Invites { get; set; } = new HashSet<Invite>();


    }
}
