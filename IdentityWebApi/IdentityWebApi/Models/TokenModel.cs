namespace IdentityWebApi.Models
{
    public class TokenModel
    {
        public required string Token { get; set; }

        public DateTime Created { get; set; }
    }
}
