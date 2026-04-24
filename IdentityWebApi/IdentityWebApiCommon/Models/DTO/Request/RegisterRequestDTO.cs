using System.ComponentModel.DataAnnotations;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    public class RegisterRequestDTO
    {
        [Required]
        [MaxLength(10)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(5)]
        public string? Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? Suffix { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string EmailAddress { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string? Phone { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string? UserRole { get; set; } = string.Empty;
    }
}
