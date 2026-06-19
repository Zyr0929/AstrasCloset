using System.ComponentModel.DataAnnotations;

namespace ProjectAstra.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string AstraId { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string ContactNumber { get; set; } = string.Empty;
        public string? FacebookLink { get; set; }

        public string? TelegramUsername { get; set; }
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
    }
}