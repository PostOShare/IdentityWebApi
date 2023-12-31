using System.ComponentModel.DataAnnotations;

namespace IdentityWebApi.Models.DTO
{
    public class UpdateRequestDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string EmailAddress { get; set; } = string.Empty;
    }
}

