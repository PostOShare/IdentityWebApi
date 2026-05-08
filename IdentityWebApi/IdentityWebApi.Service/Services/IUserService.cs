using IdentityWebApiCommon.Models.DTO;
using IdentityWebApiCommon.Models.DTO.Request;

namespace IdentityWebApi.Services
{
    public interface IUserService
    {
        Task<IdentityWebApiCommon.Models.DTO.Response.UserDTO> ListUserData(ListUserDataRequestDTO listUserDataRequestDTO);
    }
}