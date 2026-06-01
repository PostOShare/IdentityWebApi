using System.Text.Json.Serialization;

namespace IdentityWebApiCommon.Models.DTO.Response
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class LearnDetailDTO
    {
        [JsonPropertyName("institutionName")]
        public string? InstitutionName { get; set; }

        [JsonPropertyName("major")]
        public string? Major { get; set; }

        [JsonPropertyName("award")]
        public string? Award { get; set; }

        [JsonPropertyName("startYear")]
        public int StartYear { get; set; }

        [JsonPropertyName("endYear")]
        public int EndYear { get; set; }
    }
}