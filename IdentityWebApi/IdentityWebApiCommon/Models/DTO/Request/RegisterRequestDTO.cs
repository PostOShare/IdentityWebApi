using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class RegisterRequestDTO
    {
        [Required]
        [MaxLength(10)]
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [MaxLength(5)]
        [JsonPropertyName("title")]
        public string? Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(10)]
        [JsonPropertyName("suffix")]
        public string? Suffix { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; } = string.Empty;
        
        [MaxLength(10)]
        [JsonPropertyName("phone")]
        public string? Phone { get; set; } = string.Empty;
        
        [MaxLength(10)]
        [JsonPropertyName("userRole")]
        public string? UserRole { get; set; } = string.Empty;
    }
}
