using IdentityWebApiCommon.Models.DTO.Request;
using IdentityWebApiCommon.Models.DTO.Response;

namespace IdentityWebApi.Services
{
    public interface IIdentityService
    {
        Task<BaseResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<BaseResponseDTO> Register(RegisterRequestDTO registerRequestDTO);
        Task<BaseResponseDTO> UserData(UserDataRequestDTO userDataRequestDTO);
        Task<BaseResponseDTO> SendVerification(UpdateRequestDTO updateRequestDTO);
        Task<BaseResponseDTO> ValidatePasscode(ValidatePasscodeRequestDTO validatePasscodeRequestDTO);
        Task<BaseResponseDTO> UpdateKeySalt(LoginRequestDTO updateRequestDTO);
        Task<AuthResultDTO> GenerateAccessToken(CreateTokenRequestDTO createTokenRequestDTO);
        Task<AuthResultDTO> ValidateAccessToken(ValidateTokenRequestDTO validateTokenRequestDTO);
    }
}