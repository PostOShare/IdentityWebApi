using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class CreateTokenRequestDTO
    {
        [Required]
        [JsonPropertyName("currentUserId")]
        public string CurrentUserId { get; set; } = string.Empty;
    }
}