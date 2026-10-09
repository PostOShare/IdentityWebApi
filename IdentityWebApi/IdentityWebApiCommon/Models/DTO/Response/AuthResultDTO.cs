namespace IdentityWebApiCommon.Models.DTO.Response
{
    public class AuthResultDTO
    {
        public string AccessToken { get; set; } = string.Empty;

        public bool Result { get; set; }

        public string ErrorCode { get; set; } = string.Empty;

        public string ErrorDescription { get; set; } = string.Empty;
    }
}