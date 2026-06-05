using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class UpdateRequestDTO
    {
        [Required]
        [MaxLength(10)]
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(6)]
        [JsonPropertyName("otp")]
        public decimal Otp { get; set; } = 000000;

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
}

