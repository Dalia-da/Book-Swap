using System.ComponentModel.DataAnnotations.Schema;

namespace BookSwap.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime SentDate { get; set; }

        [ForeignKey("SenderId")]
        public ApplicationUser Sender { get; set; }
        [ForeignKey("ReceiverId")]
        public ApplicationUser Receiver { get; set; }
    }
}