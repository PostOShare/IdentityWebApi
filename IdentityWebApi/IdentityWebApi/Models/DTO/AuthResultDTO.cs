namespace IdentityWebApi.Models.DTO
{
    public class AuthResultDTO
    {
        public string RefreshToken { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;

        public bool Result { get; set; }

        public string Error { get; set; } = string.Empty;
    }
}
