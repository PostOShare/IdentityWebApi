namespace IdentityWebApiCommon.Models.DTO.Response
{
    public class PersonalDetailDTO
    {
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? LanguageOne { get; set; }
        public string? LanguageTwo { get; set; }
        public string? Location { get; set; }
        public string? Status { get; set; }
    }
}