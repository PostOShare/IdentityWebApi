using EntityORM.DatabaseEntity;
using IdentityWebApiCommon.HelperUtility;
using IdentityWebApiCommon.Models.DTO.Request;
using IdentityWebApiCommon.Models.DTO.Response;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using UserDTO = IdentityWebApiCommon.Models.DTO.Response.UserDTO;

namespace IdentityWebApi.Services
{
    public class UserService : IUserService
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

            // Check whether the username and token exist and are valid

            var valid = await ValidateUserAndAccessToken(listUserDataRequestDTO.Username, listUserDataRequestDTO.RefreshToken, listUserDataRequestDTO.AccessToken);

            if (!valid)
            {
                _logger.LogInformation("Route: {method}, User: {username} | User is not available",
                                  Constants.ListUserDataRoute, listUserDataRequestDTO.Username);
                return new UserDTO
                {
                    Result = false,
                    Error = Constants.UsernameTokenError
                };
            }

            var userDto = await _context.Users
                .Where(u => u.Username == listUserDataRequestDTO.Username)
                .Select(u => new UserDTO
                {
                    Username = u.Username,
                    PersonalDetail = new PersonalDetailDTO
                    {
                        BirthDate = u.UserPersonalDetails.BirthDate,
                        Gender = u.UserPersonalDetails.Gender,
                        LanguageOne = u.UserPersonalDetails.LanguageOne,
                        LanguageTwo = u.UserPersonalDetails.LanguageTwo,
                        Location = u.UserPersonalDetails.Location,
                        Status = u.UserPersonalDetails.Status
                    },
                    EmploymentDetail = u.UserEmploymentDetails.Select(e => new EmploymentDetailDTO
                    {
                        EmployerName = e.EmployerName,
                        EmployerCity = e.EmployerCity,
                        IsCurrentEmployer = e.IsCurrentEmployer,
                        Role = e.Role
                    }).ToList(),
                    LearnDetail = u.UserLearnDetails.Select(l => new LearnDetailDTO
                    {
                        InstitutionName = l.InstitutionName,
                        Major = l.Major,
                        Award = l.Award,
                        StartYear = l.StartYear,
                        EndYear = l.EndYear
                    }).ToList(),
                    Result = true,
                    Error = string.Empty
                })
                .FirstOrDefaultAsync();

            _logger.LogInformation("Route: {method}, User: {username} | User data queried",
                                  Constants.ListUserDataRoute, listUserDataRequestDTO.Username);

            return userDto!;
        }

        

        public async Task<bool> ValidateUserAndAccessToken(string username, string refreshToken, string accessToken)
        {
            // Check whether the username and token exist
            UserAuth? current = null;
            try
            {
                current = await _context.UserAuths.Where(user => user.Username.Equals(username) &&
                                                         user.Token!.Equals(refreshToken))
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
                                 Constants.ListUserDataRoute, username);
                return false;
            }

            //validate access token
            var validateAccessTokenEndpointUrl = $"{_configuration["IdentityAPIUrl"]}/{Constants.ValidateAccessTokenIdentityRoute}";
            var valid = false;
            try
            {
                var client = _clientFactory.CreateClient("InternalApi");
                var data = new CreateTokenRequestDTO { AccessToken = accessToken, RefreshToken = refreshToken };
                using var response = await client.PostAsJsonAsync(validateAccessTokenEndpointUrl, data);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResultDTO>();
                    if (result!.Result)
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

            return valid;
        }

        public async Task<BaseResponseDTO> UpsertUserData(SaveUserRequestDTO saveUserDataRequest)
        {
            _logger.LogInformation("Route: {method}, User: {username} | Checking whether user exists",
                                   Constants.UpsertUserDataRoute, saveUserDataRequest.Username);

            // Check whether the username and token exist and are valid

            var valid = await ValidateUserAndAccessToken(saveUserDataRequest.Username, saveUserDataRequest.RefreshToken!, saveUserDataRequest.AccessToken!);

            if (!valid)
            {
                _logger.LogInformation("Route: {method}, User: {username} | User is not available",
                                   Constants.UpsertUserDataRoute, saveUserDataRequest.Username);

                return new BaseResponseDTO
                {
                    Result = false,
                    Error = Constants.UsernameTokenError
                };
            }

            try
            {
                User? user = await _context.Users
                                           .FirstOrDefaultAsync(u => u.Username == saveUserDataRequest.Username);

                if (user == null)
                {
                    return new BaseResponseDTO { Result = false, Error = "User not found." };
                }

                _logger.LogInformation("Route: {method}, User: {username} | Saving user's data",
                                   Constants.UpsertUserDataRoute, saveUserDataRequest.Username);

                var json = System.Text.Json.JsonSerializer.Serialize(saveUserDataRequest);

                await _context.Database.ExecuteSqlRawAsync("EXECUTE DBO.UPSERT_USERPROFILEFROMJSON @JSONDATA",
                    new SqlParameter("@JSONDATA", System.Text.Json.JsonSerializer.Serialize(saveUserDataRequest)));
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                   Constants.UpsertUserDataRoute, saveUserDataRequest.Username, ex.Message);
                throw;
            }

            _logger.LogInformation("Route: {method}, User: {username} |  User data was saved successfully",
                                   Constants.UpsertUserDataRoute, saveUserDataRequest.Username);

            return new BaseResponseDTO { Error = string.Empty, Result = true };
        }
    }
}