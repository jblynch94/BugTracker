using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketHistory
    {
        //primary key
        public int Id { get; set; }
        //foregn key 
        public int TicketId { get; set; }
        public string? PropertyName { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        [Required]
        public string? UserId { get; set; }

        //Nav Properties
        public virtual Ticket? Ticket { get; set; }
        public virtual BTUser? User { get; set; }
    }
}
