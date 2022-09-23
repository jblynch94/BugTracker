using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Ticket
    {
        //primary key
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }
        [DataType(DataType.Date)]
        public DateTime? Updated { get; set; }   
        public bool Archived { get; set; }
        public bool ArchivedByProject { get; set; }
        //foregn key
        public int ProjectId { get; set; }
        //foregn key 
        public int TicketTypeId { get; set; }
        //foregn key 
        public int TicketStatusId { get; set; }
        //foregn key
        public int TicketPriorityId { get; set; }
        //forgen key 
        public string? DeveloperUserId { get; set; }
        //foregn key
        [Required]
        public string? SubmitterUserId { get; set; }

        //Nav Properties
        public virtual Project? Project { get; set; }
        public virtual TicketPriority? TicketPriority { get; set; }
        public virtual TicketType? TicketType { get; set; }
        public virtual TicketStatus? TicketStatus { get; set; }
        public virtual BTUser? DeveloperUser { get; set; }
        public virtual BTUser? SubmitterUser { get; set; }
        public virtual ICollection<TicketComment>? Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment>? TicketAttachments { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<TicketHistory>? History { get; set; } = new HashSet<TicketHistory>();
    }
}
