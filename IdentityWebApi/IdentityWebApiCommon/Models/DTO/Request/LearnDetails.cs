using System.ComponentModel.DataAnnotations;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    public class LearnDetails
    {
        [Required]
        public string InstitutionName { get; set; }

        public string Award { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Major { get; set; }
    }
}