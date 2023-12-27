namespace IdentityWebApi.Models.DTO
{
    public class AuthResultDTO
    {
        public string Token { get; set; } = string.Empty;
        public bool Result { get; set; }
        public List<string> Error { get; set; }
    }
}
