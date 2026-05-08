using System;
using System.Text.Json.Serialization;

namespace IdentityWebApiCommon.Models.DTO.Response
{
    public class LearnDetailDTO
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
        public string? InstitutionName { get; set; }
        public string? Major { get; set; }
        public string? Award { get; set; }
    }
}