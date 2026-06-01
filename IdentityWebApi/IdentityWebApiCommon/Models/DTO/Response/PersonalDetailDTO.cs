using System.Text.Json.Serialization;

namespace IdentityWebApiCommon.Models.DTO.Response
{
    [JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
    public class PersonalDetailDTO
    {
        [JsonPropertyName("birthDate")]
        public DateTime? BirthDate { get; set; }

        [JsonPropertyName("gender")]
        public string? Gender { get; set; }

        [JsonPropertyName("languageOne")]
        public string? LanguageOne { get; set; }

        [JsonPropertyName("languageTwo")]
        public string? LanguageTwo { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}