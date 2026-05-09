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

            var valid = ValidateUserAndAccessToken(listUserDataRequestDTO.Username, listUserDataRequestDTO.RefreshToken, listUserDataRequestDTO.AccessToken);

            if (!valid)
            {
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
                        BirthDate = u.UserPersonalDetail.BirthDate,
                        Gender = u.UserPersonalDetail.Gender,
                        LanguageOne = u.UserPersonalDetail.LanguageOne,
                        LanguageTwo = u.UserPersonalDetail.LanguageTwo,
                        Location = u.UserPersonalDetail.Location,
                        Status = u.UserPersonalDetail.Status
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
                        Award = l.Award
                    }).ToList(),
                    Result = true,
                    Error = string.Empty
                })
                .FirstOrDefaultAsync();

            return userDto!;
        }

        public async Task<BaseResponseDTO> SaveUserData(IdentityWebApiCommon.Models.DTO.Request.UserDTO saveUserDataRequest)
        {
            _logger.LogInformation("Route: {method}, User: {username} | Checking whether user exists",
                                   Constants.SaveUserDataRoute, saveUserDataRequest.Username);

            // Check whether the username and token exist and are valid

            var valid = ValidateUserAndAccessToken(saveUserDataRequest.Username, saveUserDataRequest.RefreshToken, saveUserDataRequest.AccessToken);

            if (!valid)
            {
                return new BaseResponseDTO
                {
                    Result = false,
                    Error = Constants.UsernameTokenError
                };
            }

            try
            {
                //get userid from username
                var user = await _context.Users.Where(u => u.Username == saveUserDataRequest.Username).FirstOrDefaultAsync();
                var userId = user!.Id;

                _context.UserPersonalDetails.Add(new UserPersonalDetail
                {
                    UserId = userId,
                    BirthDate = saveUserDataRequest.PersonalDetail!.BirthDate,
                    Gender = saveUserDataRequest.PersonalDetail!.Gender,
                    LanguageOne = saveUserDataRequest.PersonalDetail!.LanguageOne,
                    LanguageTwo = saveUserDataRequest.PersonalDetail!.LanguageTwo,
                    Location = saveUserDataRequest.PersonalDetail!.Location,
                    Status = saveUserDataRequest.PersonalDetail!.Status
                });

                if (saveUserDataRequest.EmploymentDetail != null)
                {
                    foreach (var employmentDetail in saveUserDataRequest.EmploymentDetail)
                    {
                        _context.UserEmploymentDetails.Add(new UserEmploymentDetail
                        {
                            UserId = userId,
                            EmployerName = employmentDetail.EmployerName,
                            EmployerCity = employmentDetail.EmployerCity,
                            IsCurrentEmployer = employmentDetail.IsCurrentEmployer,
                            Role = employmentDetail.Role
                        });
                    }
                }

                if (saveUserDataRequest.LearnDetail != null)
                {
                    foreach (var learnDetail in saveUserDataRequest.LearnDetail)
                    {
                        _context.UserLearnDetails.Add(new UserLearnDetail
                        {
                            UserId = userId,
                            InstitutionName = learnDetail.InstitutionName,
                            Major = learnDetail.Major,
                            Award = learnDetail.Award,
                            StartYear = learnDetail.StartYear,
                            EndYear = learnDetail.EndYear
                        });
                    }
                }

                _context.SaveChanges();
            }
            catch(DbUpdateException ex)
            {
                _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                   Constants.SaveUserDataRoute, saveUserDataRequest.Username, ex.Message);
                throw;
            }

            _logger.LogInformation("Route: {method}, User: {username} |  User data was saved successfully",
                                   Constants.SaveUserDataRoute, saveUserDataRequest.Username);

            return new BaseResponseDTO { Error = string.Empty, Result = true };
        }

        public bool ValidateUserAndAccessToken(string username, string refreshToken, string accessToken)
        {
            // Check whether the username and token exist
            UserAuth? current = null;
            try
            {
                current = _context.UserAuths.Where(user => user.Username.Equals(username) &&
                                                         user.Token!.Equals(refreshToken))
                                               .FirstOrDefault();
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
                var response = client.PostAsJsonAsync(validateAccessTokenEndpointUrl, data).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadFromJsonAsync<AuthResultDTO>().Result;
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
    }
}