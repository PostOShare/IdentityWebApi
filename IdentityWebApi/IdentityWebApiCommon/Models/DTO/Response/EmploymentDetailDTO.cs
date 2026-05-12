using System;
using System.Text.Json.Serialization;

namespace IdentityWebApiCommon.Models.DTO.Response
{
    public class EmploymentDetailDTO
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int UserId { get; set; }
        public string? EmployerName { get; set; }
        public string? EmployerCity { get; set; }
        public bool? IsCurrentEmployer { get; set; }
        public string? Role { get; set; }
    }
}