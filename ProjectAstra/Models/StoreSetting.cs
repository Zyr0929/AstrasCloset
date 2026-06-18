using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectAstra.Models
{
    public class StoreSetting
    {
        public int Id { get; set; }

        [Required]
        public string CurrentPhase { get; set; } 

        public DateTime PreorderDeadline { get; set; }
    }
}