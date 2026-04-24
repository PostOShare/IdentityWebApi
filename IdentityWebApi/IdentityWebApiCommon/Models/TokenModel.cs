using System.ComponentModel.DataAnnotations;

namespace IdentityWebApiCommon.Models
{
    public class TokenModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime Created { get; set; }
    }
}
