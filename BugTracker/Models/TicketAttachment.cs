using BugTracker.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    public class TicketAttachment
    {
        //primary key 
        public int Id { get; set; }
        public string? Description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Created { get; set; }
        //foregn key
        public int TicketId { get; set; }
        //foregn key
        [Required]
        public string? UserId { get; set; }
        
        [NotMapped]
        [DisplayName("Select a file")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1024 * 1024)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".doc", ".docx", ".xls", ".xlsx", ".pdf" })]
        public IFormFile? FormFile { get; set; }
        public byte[]? FileData { get; set; }
        public string? FileType { get; set; }

        //Nav Properties
        public virtual Ticket? Ticket { get; set; }
        public virtual BTUser? User { get; set; }
    }
}
