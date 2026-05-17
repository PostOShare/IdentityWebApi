namespace IdentityWebApiCommon.Models.DTO.Response
{
    public class LearnDetailDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? InstitutionName { get; set; }
        public string? Major { get; set; }
        public string? Award { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
    }
}