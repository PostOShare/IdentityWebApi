using System.ComponentModel.DataAnnotations;

namespace IdentityWebApi.Models.DTO
{
    public class RegisterRequestDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? Title { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        public string? Suffix { get; set; } = string.Empty;

        [Required]
        public string EmailAddress { get; set; } = string.Empty;

        public string? Phone { get; set; } = string.Empty;

        public string? UserRole { get; set; } = string.Empty;
    }
}
