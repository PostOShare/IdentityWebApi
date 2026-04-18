namespace IdentityWebApi.Models.DTO.Response
{
    public class RegisterResponseDTO
    {
        public bool Result { get; set; }

        public string Error { get; set; } = string.Empty;
    }
}
