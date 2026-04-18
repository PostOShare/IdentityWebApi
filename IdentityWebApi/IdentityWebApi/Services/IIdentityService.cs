using IdentityWebApi.Models.DTO.Request;
using IdentityWebApi.Models.DTO.Response;

namespace IdentityWebApi.Services
{
    public interface IIdentityService
    {
        Task<AuthResultDTO> Login(LoginRequestDTO login);
        Task<RegisterResponseDTO> Register(RegisterRequestDTO register);
    }
}
