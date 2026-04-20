using EntityORM.DatabaseEntity;
using IdentityWebApi.Repositories;
using IdentityWebApiCommon.HelperUtility;
using IdentityWebApiCommon.Models;
using IdentityWebApiCommon.Models.DTO.Request;
using IdentityWebApiCommon.Models.DTO.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

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

        public async Task<AuthResultDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            _logger.LogInformation("Route: {method}, User: {username} | Checking whether user exists",
                                   Constants.LoginIdentityRoute, loginRequestDTO.Username);

            // Check whether a user exists

            Login? current = null;

            try
            {
                current = await _context.Logins.Where(user => user.Username.Equals(loginRequestDTO.Username))
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
                                 Constants.LoginIdentityRoute, loginRequestDTO.Username);

                return new AuthResultDTO
                {
                    Result = false,
                    RefreshToken = string.Empty,
                    AccessToken = string.Empty,
                    Error = Constants.UserValidationError
                };
            }

            var exists = false;

            using (var deriveBytes = new Rfc2898DeriveBytes(loginRequestDTO.Password, Convert.FromBase64String(current.Salt)))
            {
                byte[] newKey = deriveBytes.GetBytes(20);
                exists = newKey.SequenceEqual(Convert.FromBase64String(current.Key));
            }

            if (!exists)
            {
                _logger.LogError("Route: {method}, User: {username} | Invalid username and/or password",
                                 Constants.LoginIdentityRoute, loginRequestDTO.Username);
                return new AuthResultDTO
                {
                    Result = false,
                    RefreshToken = string.Empty,
                    AccessToken = string.Empty,
                    Error = Constants.UserValidationError
                };
            }

            // If the user exists, update loginRequestDTO time
            current.LastLoginTime = DateTime.Now;

            try
            {
                _logger.LogInformation("Route: {method}, User: {username} | Updating date and time for current loginRequestDTO: {logintime}",
                                       Constants.LoginIdentityRoute, loginRequestDTO.Username, current.LastLoginTime);

                _context.Logins.Update(current);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                 Constants.LoginIdentityRoute, loginRequestDTO.Username, ex.Message);
                throw;
            }

            //Generate a refresh token and save in DB

            _logger.LogInformation("Route: {method}, User: {username} | Generate a refresh token and save in DB",
                                   Constants.LoginIdentityRoute, loginRequestDTO.Username);

            var refresh = new RefreshTokenGenerationHelper().GenerateRefreshToken().Token;

            _logger.LogInformation("Route: {method}, User: {username} | Checking whether loginRequestDTO exists",
                                   Constants.LoginIdentityRoute, loginRequestDTO.Username);

            UserAuth? authUser = null;

            try
            {
                authUser = await _context.UserAuths.Where(user => user.Username.Equals(loginRequestDTO.Username))
                                                   .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                throw;
            }

            if (authUser == null)
            {
                _logger.LogInformation("Route: {method}, User: {username} | New loginRequestDTO", Constants.LoginIdentityRoute,
                                       loginRequestDTO.Username);

                var userAuth = new UserAuth
                {
                    Username = current.Username,
                    Token = refresh,
                    CreatedTime = DateTime.Now,
                    Enabled = true
                };

                try
                {
                    _logger.LogInformation("Route: {method}, User: {username} | Adding new loginRequestDTO",
                                           Constants.LoginIdentityRoute, loginRequestDTO.Username);

                    _context.Add(userAuth);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                     Constants.LoginIdentityRoute, loginRequestDTO.Username, ex.Message);
                    throw;
                }
            }
            else
            {
                _logger.LogInformation("Route: {method}, User: {username} | Login exists",
                                       Constants.LoginIdentityRoute, loginRequestDTO.Username);

                authUser.Token = refresh;
                authUser.CreatedTime = DateTime.Now;

                try
                {
                    _logger.LogInformation("Route: {method}, User: {username} | Updating loginRequestDTO",
                                           Constants.LoginIdentityRoute, loginRequestDTO.Username);

                    _context.UserAuths.Update(authUser);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                     Constants.LoginIdentityRoute, loginRequestDTO.Username, ex.Message);
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

        public async Task<BaseResponseDTO> Register(RegisterRequestDTO registerRequestDTO)
        {
            _logger.LogInformation("Route: {method}, User: {username} | Checking whether user exists",
                                   Constants.RegisterIdentityRoute, registerRequestDTO.Username);

            Login? current = null;

            try
            {
                current = await _context.Logins.Where(user => user.Username.Equals(registerRequestDTO.Username))
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
                                       Constants.RegisterIdentityRoute, registerRequestDTO.Username);

                return new BaseResponseDTO { Error = "The given account could not be registered." , Result = false};
            }

            //If the user does not exist, add the user with the given data to Login

            _logger.LogInformation("Route: {method}, User: {username} |  Add the user to Login",
                                   Constants.RegisterIdentityRoute, registerRequestDTO.Username);

            using (var deriveBytes = new Rfc2898DeriveBytes(registerRequestDTO.Password, 20))
            {
                var login = new Login
                {
                    Username = registerRequestDTO.Username,
                    Salt = Convert.ToBase64String(deriveBytes.Salt),
                    Key = Convert.ToBase64String(deriveBytes.GetBytes(20)),
                    RegisteredDate = DateTime.Now,
                    LastLoginTime = DateTime.Now,
                    UserRole = registerRequestDTO.UserRole,
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
                                        Constants.RegisterIdentityRoute, registerRequestDTO.Username, ex.Message);
                    throw;
                }
            }

            _logger.LogInformation("Route: {method}, User: {username} |  Add the user to User",
                                   Constants.RegisterIdentityRoute, registerRequestDTO.Username);

            var user = new User
            {
                Username = registerRequestDTO.Username,
                Title = registerRequestDTO.Title,
                FirstName = registerRequestDTO.FirstName,
                LastName = registerRequestDTO.LastName,
                Suffix = registerRequestDTO.Suffix,
                EmailAddress = registerRequestDTO.EmailAddress,
                Phone = registerRequestDTO.Phone
            };

            try
            {
                _context.Add(user);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                    Constants.RegisterIdentityRoute, registerRequestDTO.Username, ex.Message);
                throw;
            }

            _logger.LogInformation("Route: {method}, User: {username} |  User has been registered",
                                   Constants.RegisterIdentityRoute, registerRequestDTO.Username);

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

        public async Task<AuthResultDTO> GenerateAccessToken(CreateTokenRequestDTO createTokenRequestDTO)
        {
            _logger.LogInformation("Route: {method}, Refresh token: {token} | Checking whether the refresh token exists",
                                   Constants.GenerateAccessTokenIdentityRoute, createTokenRequestDTO.RefreshToken);

            UserAuth? authUser = null;
            try
            {
                authUser = await _context.UserAuths.Where(user => user.Token!.Equals(createTokenRequestDTO.RefreshToken))
                                                   .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                throw;
            }

            if (authUser == null)
            {
                _logger.LogError("Route: {method}, Refresh token: {token} | Invalid refresh token",
                                 Constants.GenerateAccessTokenIdentityRoute, createTokenRequestDTO.RefreshToken);

                return new AuthResultDTO
                {
                    RefreshToken = string.Empty,
                    AccessToken = string.Empty,
                    Result = false,
                    Error = Constants.InvalidRefreshTokenError
                };
            }

            var refresh = authUser.Token;

            // if the refresh token's created time is more than 1 day it is expired
            if (authUser.CreatedTime.AddDays(1) > DateTime.UtcNow)
            {
                _logger.LogInformation("Route: {method}, Refresh token: {token} | Generating refresh token",
                                  Constants.GenerateAccessTokenIdentityRoute, createTokenRequestDTO.RefreshToken);

                refresh = new RefreshTokenGenerationHelper().GenerateRefreshToken().Token;
                authUser.CreatedTime = DateTime.Now;

                try
                {
                    _context.UserAuths.Update(authUser);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);
                    throw;
                }
            }

            _logger.LogInformation("Route: {method}, Refresh token: {token} | Generating access token",
                                  Constants.GenerateAccessTokenIdentityRoute, createTokenRequestDTO.RefreshToken);

            var access = new JWTTokenGenerationHelper().GenerateJWTToken(authUser.Username);

            _logger.LogInformation("Route: {method}, Refresh token: {token} | Token(s) were created",
                                  Constants.GenerateAccessTokenIdentityRoute, createTokenRequestDTO.RefreshToken);

            return new AuthResultDTO
            {
                RefreshToken = refresh!,
                AccessToken = access,
                Result = true
            };
        }

        public async Task<AuthResultDTO> ValidateAccessToken(CreateTokenRequestDTO createTokenRequestDTO, string secretKey)
        {
            _logger.LogInformation("Route: {method}, Access token: {token} | Validating the access token",
                                   Constants.ValidateAccessTokenIdentityRoute, createTokenRequestDTO.AccessToken);

            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            JwtSecurityToken? token = null;
            try
            {
                handler.ValidateToken(createTokenRequestDTO.AccessToken, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = false
                }, out var validateToken);

                token = (JwtSecurityToken) validateToken;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("The token is invalid or unknown exception: {exception}", ex.Message);

                return new AuthResultDTO
                           {
                               Error = Constants.InvalidAccessTokenError,
                               Result = false
                           };
            }


            var expiry = Convert.ToInt64(token.Claims.Where(p => p.Type == "exp").FirstOrDefault()?.Value);
            var expired = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() > expiry;

            if (expired)
            {
                _logger.LogInformation("Route: {method}, Access token: {token} | The access token is valid",
                                   Constants.ValidateAccessTokenIdentityRoute, createTokenRequestDTO.AccessToken);

                return new AuthResultDTO
                {
                    Error = Constants.TokenExpiredError,
                    Result = false
                };
            }
            else
            {
                _logger.LogInformation("Route: {method}, Access token: {token} | The access token is expired",
                                   Constants.ValidateAccessTokenIdentityRoute, createTokenRequestDTO.AccessToken);

                return new AuthResultDTO
                {
                    Error = string.Empty,
                    Result = true
                };
            }
        }
    }
}