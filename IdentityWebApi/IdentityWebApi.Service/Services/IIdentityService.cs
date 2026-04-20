using IdentityWebApiCommon.Models.DTO.Request;
using IdentityWebApiCommon.Models.DTO.Response;

namespace IdentityWebApi.Services
{
    public interface IIdentityService
    {
        Task<AuthResultDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<BaseResponseDTO> Register(RegisterRequestDTO registerRequestDTO);
        Task<BaseResponseDTO> UserData(UpdateRequestDTO updateRequestDTO);
        Task<BaseResponseDTO> SendVerification(UpdateRequestDTO updateRequestDTO);
        Task<BaseResponseDTO> ValidatePasscode(UpdateRequestDTO updateRequestDTO);
        Task<BaseResponseDTO> UpdateKeySalt(UpdateRequestDTO updateRequestDTO);
        Task<AuthResultDTO> GenerateAccessToken(CreateTokenRequestDTO createTokenRequestDTO);
        Task<AuthResultDTO> ValidateAccessToken(CreateTokenRequestDTO createTokenRequestDTO, string secretKey);
    }
}