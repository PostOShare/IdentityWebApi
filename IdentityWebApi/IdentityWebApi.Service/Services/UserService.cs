using EntityORM.DatabaseEntity;
using IdentityWebApiCommon.HelperUtility;
using IdentityWebApiCommon.Models.DTO.Request;
using IdentityWebApiCommon.Models.DTO.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using UserDTO = IdentityWebApiCommon.Models.DTO.Response.UserDTO;

namespace IdentityWebApi.Services
{
    public class UserService: IUserService
    {
        private readonly IdentityPMContext _context;
        private readonly ILogger<UserService> _logger;
        public IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public UserService(IdentityPMContext context, ILogger<UserService> logger, IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<UserDTO> ListUserData(ListUserDataRequestDTO listUserDataRequestDTO)
        {
            _logger.LogInformation("Route: {method}, User: {username} | Checking whether user exists",
                                   Constants.ListUserDataRoute, listUserDataRequestDTO.Username);

            // Check whether the username and token exist

            UserAuth? current = null;

            try
            {
                current = await _context.UserAuths.Where(user => user.Username.Equals(listUserDataRequestDTO.Username) &&
                                                                 user.Token!.Equals(listUserDataRequestDTO.RefreshToken))
                                               .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                throw;
            }

            if (current == null)
            {
                _logger.LogError("Route: {method}, User: {username} | Invalid username and/or expired token",
                                 Constants.ListUserDataRoute, listUserDataRequestDTO.Username);

                return new UserDTO
                {
                    Result = false,
                    Error = Constants.UsernameTokenError
                };
            }

            //validate access token
            var validateAccessTokenEndpointUrl = $"{_configuration["IdentityAPIUrl"]}/{Constants.ValidateAccessTokenIdentityRoute}";
            var valid = false;

            try
            {
                var client = _clientFactory.CreateClient("InternalApi");
                var data = new CreateTokenRequestDTO { AccessToken = listUserDataRequestDTO.AccessToken, RefreshToken = listUserDataRequestDTO.RefreshToken };
                var response = await client.PostAsJsonAsync(validateAccessTokenEndpointUrl, data);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResultDTO>();
                    if(result!.Result)
                    {
                        valid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception while calling Identity API {exception}", ex.Message);
                throw;
            }

            if (!valid)
            {
                return new UserDTO
                {
                    Result = false,
                    Error = Constants.InvalidAccessTokenError
                };
            }

            // project into your existing UserDTO (use Select to let EF only fetch needed fields)
            var userDto = await _context.Users
                .Where(u => u.Username == listUserDataRequestDTO.Username)
                .Select(u => new UserDTO
                {
                    Username = u.Username,
                    PersonalDetail = new PersonalDetailDTO {
                        BirthDate = u.UserPersonalDetail.BirthDate,
                        Gender = u.UserPersonalDetail.Gender,
                        LanguageOne = u.UserPersonalDetail.LanguageOne,
                        LanguageTwo = u.UserPersonalDetail.LanguageTwo,
                        Location = u.UserPersonalDetail.Location,
                        Status = u.UserPersonalDetail.Status
                    },
                    EmploymentDetail = u.UserEmploymentDetails.Select(e => new EmploymentDetailDTO {
                        EmployerName = e.EmployerName,
                        EmployerCity = e.EmployerCity,
                        IsCurrentEmployer = e.IsCurrentEmployer,
                        Role = e.Role
                    }).ToList(),
                    LearnDetail = u.UserLearnDetails.Select(l => new LearnDetailDTO {
                        InstitutionName = l.InstitutionName,
                        Major = l.Major,
                        Award = l.Award
                    }).ToList(),
                    Result = true,
                    Error = string.Empty
                })
                .FirstOrDefaultAsync();

            return userDto!;
        }
    }
}