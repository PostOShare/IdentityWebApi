using System.ComponentModel.DataAnnotations;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    public class EmploymentDetails
    {
        [Required]
        public string EmployerName { get; set; }

        public string EmployerCity { get; set; }

        public string Role { get; set; }

        public string Responsibilities { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsCurrentEmployer { get; set; } = false;
    }
}