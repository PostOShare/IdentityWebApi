using IdentityWebApiCommon.Models.DTO.Request;
using IdentityWebApiCommon.Models.DTO.Response;

namespace IdentityWebApi.Services
{
    public interface IUserService
    {
        Task<IdentityWebApiCommon.Models.DTO.Response.UserDTO> ListUserData(ListUserDataRequestDTO listUserDataRequestDTO);
        Task<BaseResponseDTO> SaveUserData(IdentityWebApiCommon.Models.DTO.Request.UserDTO saveUserDataRequest);
    }
}