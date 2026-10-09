using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    public class ValidatePasscodeRequestDTO
    {
        [Required]
        [MaxLength(10)]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [Required]
        [Range(0, 999999)]
        [JsonPropertyName("otp")]
        public decimal Otp { get; set; }
    }
}