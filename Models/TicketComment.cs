using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class TicketComment
    {
        //primary key
        public int Id { get; set; }
        [Required]
        public string? Comment { get; set; }
        [DataType(DataType.Date)]
        public DateTime Created { get; set; }
        //foregn key 
        public int TicketId { get; set; }
        //foregn key
        [Required]
        public string? UserId { get; set; }

        //Nav Properties
        public virtual Ticket? Ticket { get; set; }
        public virtual BTUser? User { get; set; }
    }
}
