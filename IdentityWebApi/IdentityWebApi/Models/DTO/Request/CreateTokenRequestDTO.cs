using System.ComponentModel.DataAnnotations;

namespace IdentityWebApi.Models.DTO.Request
{
    public class CreateTokenRequestDTO
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
