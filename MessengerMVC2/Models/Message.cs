using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MessengerMVC2.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("Sender")]
        public string SenderId { get; set; }
        public virtual ApplicationUser Sender { get; set; }

        [ForeignKey("Receiver")]
        public string ReceiverId { get; set; }
        public virtual ApplicationUser Receiver { get; set; }
    }
}
