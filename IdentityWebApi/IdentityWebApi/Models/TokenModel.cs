using System.ComponentModel.DataAnnotations;

namespace IdentityWebApi.Models
{
    public class TokenModel
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime Created { get; set; }
    }
}
