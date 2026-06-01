using IdentityWebApiCommon.Models.DTO.Response;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class SaveUserRequestDTO
    {
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; set; } = null!;

        [Required]
        [JsonPropertyName("refreshToken")]
        public string? RefreshToken { get; set; }

        [Required]
        [JsonPropertyName("accessToken")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("personalDetail")]
        public PersonalDetailDTO? PersonalDetail { get; set; }

        [JsonPropertyName("employmentDetail")]
        public List<EmploymentDetailDTO>? EmploymentDetail { get; set; }

        [JsonPropertyName("learnDetail")]
        public List<LearnDetailDTO>? LearnDetail { get; set; }
    }
}