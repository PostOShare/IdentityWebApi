using EntityORM.DatabaseEntity;
using System.ComponentModel.DataAnnotations;

namespace IdentityWebApiCommon.Models.DTO.Request
{
    public class UserDTO
    {
        [Required]
        [MaxLength(10)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public UserPersonalDetail? PersonalDetail { get; set; }

        [Required]
        public List<UserEmploymentDetail>? EmploymentDetail { get; set; }

        [Required]
        public List<UserLearnDetail>? LearnDetail { get; set; }
    }
}