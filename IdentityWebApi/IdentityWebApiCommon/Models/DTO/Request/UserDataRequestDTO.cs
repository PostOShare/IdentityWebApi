using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    public class UserDataRequestDTO
    {
        [Required]
        [MaxLength(10)]
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; } = string.Empty;
    }
}