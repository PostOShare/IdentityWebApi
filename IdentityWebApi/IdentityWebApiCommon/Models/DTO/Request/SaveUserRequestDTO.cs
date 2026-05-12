using IdentityWebApiCommon.Models.DTO.Response;
using System.ComponentModel.DataAnnotations;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    public class SaveUserRequestDTO
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string? RefreshToken { get; set; }

        [Required]
        public string? AccessToken { get; set; }

        public PersonalDetailDTO? PersonalDetail { get; set; }

        public List<EmploymentDetailDTO>? EmploymentDetail { get; set; }

        public List<LearnDetailDTO>? LearnDetail { get; set; }
    }
}