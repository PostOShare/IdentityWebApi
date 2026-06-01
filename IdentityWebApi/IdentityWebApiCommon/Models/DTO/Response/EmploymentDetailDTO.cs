using System.Text.Json.Serialization;

namespace IdentityWebApiCommon.Models.DTO.Response
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class EmploymentDetailDTO
    {
        [JsonPropertyName("employerName")]
        public string? EmployerName { get; set; }

        [JsonPropertyName("employerCity")]
        public string? EmployerCity { get; set; }

        [JsonPropertyName("isCurrentEmployer")]
        public bool? IsCurrentEmployer { get; set; }

        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("responsibilities")]
        public string? Responsibilities { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }
    }
}