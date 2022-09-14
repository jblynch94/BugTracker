using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class TicketAttachment
    {
        //primary key 
        public int Id { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        //foregn key
        public int TicketId { get; set; }
        //foregn key
        [Required]
        public string? UserId { get; set; }
        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile? FormFile { get; set; }
        public byte[]? FileDate { get; set; }
        public string? FileType { get; set; }

        //Nav Properties
        public virtual Ticket? Ticket { get; set; }
        public virtual BTUser? User { get; set; }
    }
}
