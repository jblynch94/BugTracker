using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Notification
    {
        //primary key
        public int Id { get; set; }
        //foregn key
        public int ProjectId { get; set; }
        //foregn key
        public int TicketId { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Message { get; set; }
        public DateTime Created { get; set; }
        //foregn key
        public string? SenderId { get; set; }
        //foregn key
        public string? RecipentId { get; set; }
        public int NotificationTypeId { get; set; }
        public bool HasBeenViewed { get; set; }

        //Nav Properties
        public virtual NotificationType? NotificationType { get; set; }
        public virtual Ticket? Ticket { get; set; }
        public virtual Project? Project { get; set; }
        public virtual BTUser? Sender { get; set; } 
        public virtual BTUser? Recipient { get; set; }
    }
}
