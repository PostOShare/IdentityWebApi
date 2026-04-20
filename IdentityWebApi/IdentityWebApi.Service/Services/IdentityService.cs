using EntityORM.DatabaseEntity;
using IdentityWebApi.Repositories;
using IdentityWebApiCommon.HelperUtility;
using IdentityWebApiCommon.Models;
using IdentityWebApiCommon.Models.DTO.Request;
using IdentityWebApiCommon.Models.DTO.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace IdentityWebApi.Services
{
    public class IdentityService: IIdentityService
    {
        private readonly IdentityPmContext _context;
        private readonly IEmailRepository _mailService;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(IdentityPmContext context, IEmailRepository mailService, ILogger<IdentityService> logger)
        {
            _context = context;
            _mailService = mailService;
            _logger = logger;
        }

        public async Task<AuthResultDTO> Login(LoginRequestDTO login)
        {
            _logger.LogInformation("Route: {method}, User: {username} | Checking whether user exists",
                                   Constants.LoginIdentityRoute, login.Username);

            // Check whether a user exists

            Login? current = null;

            try
            {
                current = await _context.Logins.Where(user => user.Username.Equals(login.Username))
                                               .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                throw;
            }

            if (current == null)
            {
                _logger.LogError("Route: {method}, User: {username} | Invalid username and/or password",
                                 Constants.LoginIdentityRoute, login.Username);

                return new AuthResultDTO
                {
                    Result = false,
                    RefreshToken = string.Empty,
                    AccessToken = string.Empty,
                    Error = Constants.UserValidationError
                };
            }

            var exists = false;

            using (var deriveBytes = new Rfc2898DeriveBytes(login.Password, Convert.FromBase64String(current.Salt)))
            {
                byte[] newKey = deriveBytes.GetBytes(20);
                exists = newKey.SequenceEqual(Convert.FromBase64String(current.Key));
            }

            if (!exists)
            {
                _logger.LogError("Route: {method}, User: {username} | Invalid username and/or password",
                                 Constants.LoginIdentityRoute, login.Username);
                return new AuthResultDTO
                {
                    Result = false,
                    RefreshToken = string.Empty,
                    AccessToken = string.Empty,
                    Error = Constants.UserValidationError
                };
            }

            // If the user exists, update login time
            current.LastLoginTime = DateTime.Now;

            try
            {
                _logger.LogInformation("Route: {method}, User: {username} | Updating date and time for current login: {logintime}",
                                       Constants.LoginIdentityRoute, login.Username, current.LastLoginTime);

                _context.Logins.Update(current);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                 Constants.LoginIdentityRoute, login.Username, ex.Message);
                throw;
            }

            //Generate a refresh token and save in DB

            _logger.LogInformation("Route: {method}, User: {username} | Generate a refresh token and save in DB",
                                   Constants.LoginIdentityRoute, login.Username);

            var refresh = new RefreshTokenGenerationHelper().GenerateRefreshToken().Token;

            _logger.LogInformation("Route: {method}, User: {username} | Checking whether login exists",
                                   Constants.LoginIdentityRoute, login.Username);

            UserAuth? authUser = null;

            try
            {
                authUser = await _context.UserAuths.Where(user => user.Username.Equals(login.Username))
                                                   .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                throw;
            }

            if (authUser == null)
            {
                _logger.LogInformation("Route: {method}, User: {username} | New login", Constants.LoginIdentityRoute,
                                       login.Username);

                var userAuth = new UserAuth
                {
                    Username = current.Username,
                    Token = refresh,
                    CreatedTime = DateTime.Now,
                    Enabled = true
                };

                try
                {
                    _logger.LogInformation("Route: {method}, User: {username} | Adding new login",
                                           Constants.LoginIdentityRoute, login.Username);

                    _context.Add(userAuth);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                     Constants.LoginIdentityRoute, login.Username, ex.Message);
                    throw;
                }
            }
            else
            {
                _logger.LogInformation("Route: {method}, User: {username} | Login exists",
                                       Constants.LoginIdentityRoute, login.Username);

                authUser.Token = refresh;
                authUser.CreatedTime = DateTime.Now;

                try
                {
                    _logger.LogInformation("Route: {method}, User: {username} | Updating login",
                                           Constants.LoginIdentityRoute, login.Username);

                    _context.UserAuths.Update(authUser);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                     Constants.LoginIdentityRoute, login.Username, ex.Message);
                    throw;
                }

            }

            return new AuthResultDTO
            {
                Result = true,
                RefreshToken = refresh,
                AccessToken = new JWTTokenGenerationHelper().GenerateJWTToken(current.Username)
            };
        }

        public async Task<BaseResponseDTO> Register(RegisterRequestDTO register)
        {
            _logger.LogInformation("Route: {method}, User: {username} | Checking whether user exists",
                                   Constants.RegisterIdentityRoute, register.Username);

            Login? current = null;

            try
            {
                current = await _context.Logins.Where(user => user.Username.Equals(register.Username))
                                               .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                throw;
            }

            if (current != null)
            {
                _logger.LogInformation("Route: {method}, User: {username} | User exists",
                                       Constants.RegisterIdentityRoute, register.Username);

                return new BaseResponseDTO { Error = "The given account could not be registered." , Result = false};
            }

            //If the user does not exist, add the user with the given data to Login

            _logger.LogInformation("Route: {method}, User: {username} |  Add the user to Login",
                                   Constants.RegisterIdentityRoute, register.Username);

            using (var deriveBytes = new Rfc2898DeriveBytes(register.Password, 20))
            {
                var login = new Login
                {
                    Username = register.Username,
                    Salt = Convert.ToBase64String(deriveBytes.Salt),
                    Key = Convert.ToBase64String(deriveBytes.GetBytes(20)),
                    RegisteredDate = DateTime.Now,
                    LastLoginTime = DateTime.Now,
                    UserRole = register.UserRole,
                    IsActive = true
                };

                try
                {
                    _context.Add(login);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                        Constants.RegisterIdentityRoute, register.Username, ex.Message);
                    throw;
                }
            }

            _logger.LogInformation("Route: {method}, User: {username} |  Add the user to User",
                                   Constants.RegisterIdentityRoute, register.Username);

            var user = new User
            {
                Username = register.Username,
                Title = register.Title,
                FirstName = register.FirstName,
                LastName = register.LastName,
                Suffix = register.Suffix,
                EmailAddress = register.EmailAddress,
                Phone = register.Phone
            };

            try
            {
                _context.Add(user);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                    Constants.RegisterIdentityRoute, register.Username, ex.Message);
                throw;
            }

            _logger.LogInformation("Route: {method}, User: {username} |  User has been registered",
                                   Constants.RegisterIdentityRoute, register.Username);

            return new BaseResponseDTO { Error = string.Empty, Result = true };
        }

        public async Task<BaseResponseDTO> UserData(UpdateRequestDTO updateRequestDTO)
        {
            _logger.LogInformation("Route: {method}, User: {username} | Checking whether the user exists",
                                   Constants.SearchIdentityRoute, updateRequestDTO.Username);

            // Check whether a user with the username exists
            try
            {
                var username = await _context.Logins.Where(user => user.Username.Equals(updateRequestDTO.Username))
                                                    .FirstOrDefaultAsync();
                var email = await _context.Users.Where(user => user.EmailAddress.Equals(updateRequestDTO.EmailAddress))
                                                .FirstOrDefaultAsync();

                if (username == null || email == null)
                {
                    _logger.LogError("Route: {method}, User: {username} | Invalid username and/or email",
                                     Constants.SearchIdentityRoute, updateRequestDTO.Username);

                    return new BaseResponseDTO { Error = Constants.UserValidationError, Result = false };
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                throw;
            }

            _logger.LogInformation("Route: {method}, User: {username} |  User exists",
                                  Constants.SearchIdentityRoute, updateRequestDTO.Username);

            return new BaseResponseDTO { Error = string.Empty, Result = true };
        }

        public async Task<BaseResponseDTO> SendVerification(UpdateRequestDTO updateRequestDTO)
        {
            _logger.LogInformation("Route: {method}, User: {username} | Checking whether the user exists",
                                   Constants.VerifyIdentityRoute, updateRequestDTO.Username);

            Otpvalidate? user = null;
            try
            {
                user = await _context.Otpvalidates.Where(user => user.Username.Equals(updateRequestDTO.Username))
                                                  .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                throw;
            }

            if (user == null)
            {
                _logger.LogError("Route: {method}, User: {username} | Invalid username",
                                 Constants.VerifyIdentityRoute, updateRequestDTO.Username);
                return new BaseResponseDTO { Error = Constants.UserValidationError, Result = false };
            }

            var otpModel = new OTPModel();
            var otp = otpModel.CreateOTP();

            var otpvalidate = new Otpvalidate
            {
                Username = updateRequestDTO.Username,
                Otp = otp,
                RequestedTime = DateTime.UtcNow,
                RetryAttempt = 0
            };

            try
            {
                _context.Otpvalidates.Add(otpvalidate);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                throw;
            }

            var body = otpModel.Body(otp);

            _logger.LogInformation("Route: {method}, User: {username} | Sending OTP to email",
                                   Constants.VerifyIdentityRoute, updateRequestDTO.Username);

            var sent = await _mailService.SendMail(updateRequestDTO.EmailAddress, Constants.Subject, body);

            if (sent)
            {
                _logger.LogInformation("Route: {method}, User: {username} | OTP was sent",
                                       Constants.VerifyIdentityRoute, updateRequestDTO.Username);
                return new BaseResponseDTO { Error = string.Empty, Result = true };
            }
            else
            {
                _logger.LogInformation("Route: {method}, User: {username} | A server error occurred",
                                       Constants.VerifyIdentityRoute, updateRequestDTO.Username);
                throw new Exception("An internal error occurred while sending OTP");
            }
        }

        public async Task<BaseResponseDTO> ValidatePasscode(UpdateRequestDTO updateRequestDTO)
        {
            _logger.LogInformation("Route: {method}, User: {username} | Checking whether the user exists",
                                   Constants.ValidatePasscodeIdentityRoute, updateRequestDTO.Username);

            Otpvalidate? exist = null;
            try
            {
                exist = await _context.Otpvalidates.Where(user => user.Username.Equals(updateRequestDTO.Username))
                                                   .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                throw;
            }

            if (exist == null)
            {
                _logger.LogError("Route: {method}, User: {username} | Invalid username",
                                 Constants.ValidatePasscodeIdentityRoute, updateRequestDTO.Username);

                return new BaseResponseDTO { Error = Constants.UserValidationError, Result = false };
            }

            if (exist.Otp == updateRequestDTO.Otp)
            {
                _logger.LogInformation("Route: {method}, User: {username} | OTP entered is valid",
                                       Constants.ValidatePasscodeIdentityRoute, updateRequestDTO.Username);

                return new BaseResponseDTO { Error = string.Empty, Result = true };
            }
            else
            {
                _logger.LogInformation("Route: {method}, User: {username} | OTP entered is invalid",
                                       Constants.ValidatePasscodeIdentityRoute, updateRequestDTO.Username);

                if (exist.RetryAttempt < 4)
                {
                    _logger.LogInformation("Route: {method}, User: {username} | Updating the request attempt",
                                           Constants.ValidatePasscodeIdentityRoute, updateRequestDTO.Username);

                    exist.RetryAttempt++;

                    try
                    {
                        _context.Otpvalidates.Update(exist);
                        _context.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                        throw;
                    }

                    return new BaseResponseDTO { Error = Constants.InvalidOTPError, Result = false };
                }
                else
                {
                    _logger.LogInformation("Route: {method}, User: {username} | Maximum attempts have been attempted",
                                           Constants.ValidatePasscodeIdentityRoute, updateRequestDTO.Username);
                    return new BaseResponseDTO { Error = "Cannot try more than maximum attempts", Result = false };
                }
            }
        }

        public async Task<BaseResponseDTO> UpdateKeySalt(UpdateRequestDTO updateRequestDTO)
        {
            _logger.LogInformation("Route: {method}, User: {username} | Checking whether the user exists",
                                   Constants.ChangeCredentialsIdentityRoute, updateRequestDTO.Username);

            Login? current = null;
            try
            {
                current = await _context.Logins.Where(user => user.Username.Equals(updateRequestDTO.Username))
                                               .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                throw;
            }

            if (current == null)
            {
                _logger.LogError("Route: {method}, User: {username} | Invalid username",
                                 Constants.ChangeCredentialsIdentityRoute, updateRequestDTO.Username);

                return new BaseResponseDTO { Error = Constants.UserValidationError, Result = false };
            }

            _logger.LogInformation("Route: {method}, User: {username} | Updating Key and Salt",
                                   Constants.ChangeCredentialsIdentityRoute, updateRequestDTO.Username);

            using (var deriveBytes = new Rfc2898DeriveBytes(updateRequestDTO.Password, 20))
            {
                current.Salt = Convert.ToBase64String(deriveBytes.Salt);
                current.Key = Convert.ToBase64String(deriveBytes.GetBytes(20));

                try
                {
                    _context.Update(current);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                    throw;
                }
            }

            _logger.LogInformation("Route: {method}, User: {username} | Key and Salt were updated",
                                   Constants.ChangeCredentialsIdentityRoute, updateRequestDTO.Username);

            return new BaseResponseDTO { Error = string.Empty, Result = true };
        }
    }
}