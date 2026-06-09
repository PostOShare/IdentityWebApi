using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class ListUserDataRequestDTO
    {
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [Required]
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [Required]
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
    }
}