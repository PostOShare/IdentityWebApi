using System.ComponentModel.DataAnnotations;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    public class ListUserDataRequestDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; }

        [Required]
        public string AccessToken { get; set; }
    }
}