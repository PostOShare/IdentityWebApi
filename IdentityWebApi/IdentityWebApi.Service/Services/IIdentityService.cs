using IdentityWebApiCommon.Models.DTO.Request;
using IdentityWebApiCommon.Models.DTO.Response;

namespace IdentityWebApi.Services
{
    public interface IIdentityService
    {
        Task<AuthResultDTO> Login(LoginRequestDTO login);
        Task<BaseResponseDTO> Register(RegisterRequestDTO register);
        Task<BaseResponseDTO> UserData(UpdateRequestDTO updateRequestDTO);
        Task<BaseResponseDTO> SendVerification(UpdateRequestDTO updateRequestDTO);
        Task<BaseResponseDTO> ValidatePasscode(UpdateRequestDTO updateRequestDTO);
        Task<BaseResponseDTO> UpdateKeySalt(UpdateRequestDTO updateRequestDTO);
    }
}