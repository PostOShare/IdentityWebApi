namespace IdentityWebApiCommon.Models.DTO.Response
{
    public class UserDTO
    {
        public string? Username { get; set; }
        public PersonalDetailDTO? PersonalDetail { get; set; }
        public List<EmploymentDetailDTO>? EmploymentDetail { get; set; }
        public List<LearnDetailDTO>? LearnDetail { get; set; }
        public bool Result { get; set; }
        public string? Error { get; set; }
    }
}