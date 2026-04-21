using System.ComponentModel.DataAnnotations;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    public class UpdateRequestDTO
    {
        [Required]
        [MaxLength(10)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(6)]
        public decimal Otp { get; set; } = 000000;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

