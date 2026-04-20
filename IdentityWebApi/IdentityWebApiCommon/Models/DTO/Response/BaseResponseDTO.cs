namespace IdentityWebApiCommon.Models.DTO.Response
{
    public class BaseResponseDTO
    {
        public bool Result { get; set; }

        public string Error { get; set; } = string.Empty;
    }
}
