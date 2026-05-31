namespace IdentityWebApiCommon.Models.DTO.Response
{
    public class EmploymentDetailDTO
    {
        public string? EmployerName { get; set; }
        public string? EmployerCity { get; set; }
        public bool? IsCurrentEmployer { get; set; }
        public string? Role { get; set; }
        public string? Responsibilities { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}