using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class LoginRequestDTO
    {
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("registeredDate")]
        public DateTime RegisteredDate { get; set; } = DateTime.Now;

        [JsonPropertyName("lastLoginTime")]
        public DateTime LastLoginTime { get; set; } = DateTime.Now;

        [JsonPropertyName("userRole")]
        public string? UserRole { get; set; } = string.Empty;

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = false;
    }
}
