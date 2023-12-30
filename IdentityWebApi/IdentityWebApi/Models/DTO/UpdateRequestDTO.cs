using System.ComponentModel.DataAnnotations;

namespace IdentityWebApi.Models.DTO
{
    public class UpdateRequestDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string EmailAddress { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int OTP { get; set; } = 0000;
    }
}

