using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Invite
    {
        //primary key
        public int Id { get; set; }
        public DateTime InviteDate { get; set; }
        public DateTime? JoinDate { get; set; }
        public Guid CompanyToken { get; set; }
        //foregn key
        public int CompanyId { get; set; }
        //foregn key
        public int ProjectId { get; set; }
        //foregn key
        [Required]
        public string? InvitorId { get; set; }
        //foregn key
        public string? InviteeId { get; set; }
        [Required]
        public string? InviteeEmail { get; set; }
        [Required]
        public string? InviteeFirstName { get; set; }
        [Required]
        public string? InviteeLastName { get; set; }
        public string? Message { get; set; }
        public bool IsValid { get; set; }

        //Nav Properties
        public virtual Company? Company { get; set; }
        public virtual Project? Project { get; set; }
        public virtual BTUser? Invitor { get; set; }
        public virtual BTUser? Invitee { get; set; }
    }
}
