using System.ComponentModel.DataAnnotations;

namespace IdentityWebApi.Models.DTO
{
    public class LoginRequestDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public DateTime RegisteredDate { get; set; } = DateTime.Now;

        public DateTime LastLoginTime { get; set; } = DateTime.Now;

        public string? UserRole { get; set; } = string.Empty;

        public bool IsActive { get; set; } = false;
    }
}
