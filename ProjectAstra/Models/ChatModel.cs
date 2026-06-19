using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectAstra.Models
{
    public class ChatMessage
    {
        [Key] public int Id { get; set; }
        [Required] public string SenderUsername { get; set; } = string.Empty;
        [Required] public string ReceiverUsername { get; set; } = string.Empty;
        [Required] public string MessageText { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
    }
}