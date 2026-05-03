namespace IdentityWebApiCommon.Models.DTO.Request
{
    public class PersonalDetails
    {
        public string Location { get; set; }

        public DateTime BirthDate { get; set; }

        public string Status { get; set; }

        public string Gender { get; set; }

        public string LanguageOne { get; set; }

        public string LanguageTwo { get; set; }
    }
}