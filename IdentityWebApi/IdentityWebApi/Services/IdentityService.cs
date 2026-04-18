using EntityORM.DatabaseEntity;
using IdentityWebApi.HelperUtility;
using IdentityWebApi.Models;
using IdentityWebApi.Models.DTO.Request;
using IdentityWebApi.Models.DTO.Response;
using IdentityWebApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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

        public async Task<AuthResultDTO> Login(LoginRequestDTO login)
        {
            _logger.LogInformation("Route: {method}, User: {username} | Checking whether user exists",
                                   Constants.LOGINIDENTITYROUTE, login.Username);

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
                                 Constants.LOGINIDENTITYROUTE, login.Username);

                return new AuthResultDTO
                {
                    Result = false,
                    RefreshToken = string.Empty,
                    AccessToken = string.Empty,
                    Error = Constants.USERVALIDATIONERROR
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
                                 Constants.LOGINIDENTITYROUTE, login.Username);
                return new AuthResultDTO
                {
                    Result = false,
                    RefreshToken = string.Empty,
                    AccessToken = string.Empty,
                    Error = Constants.USERVALIDATIONERROR
                };
            }

            // If the user exists, update login time
            current.LastLoginTime = DateTime.Now;

            try
            {
                _logger.LogInformation("Route: {method}, User: {username} | Updating date and time for current login: {logintime}",
                                       Constants.LOGINIDENTITYROUTE, login.Username, current.LastLoginTime);

                _context.Logins.Update(current);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                 Constants.LOGINIDENTITYROUTE, login.Username, ex.Message);
                throw;
            }

            //Generate a refresh token and save in DB

            _logger.LogInformation("Route: {method}, User: {username} | Generate a refresh token and save in DB",
                                   Constants.LOGINIDENTITYROUTE, login.Username);

            var refresh = new RefreshTokenGenerationHelper().GenerateRefreshToken().Token;

            _logger.LogInformation("Route: {method}, User: {username} | Checking whether login exists",
                                   Constants.LOGINIDENTITYROUTE, login.Username);

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
                _logger.LogInformation("Route: {method}, User: {username} | New login", Constants.LOGINIDENTITYROUTE,
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
                                           Constants.LOGINIDENTITYROUTE, login.Username);

                    _context.Add(userAuth);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                     Constants.LOGINIDENTITYROUTE, login.Username, ex.Message);
                    throw;
                }
            }
            else
            {
                _logger.LogInformation("Route: {method}, User: {username} | Login exists",
                                       Constants.LOGINIDENTITYROUTE, login.Username);

                authUser.Token = refresh;
                authUser.CreatedTime = DateTime.Now;

                try
                {
                    _logger.LogInformation("Route: {method}, User: {username} | Updating login",
                                           Constants.LOGINIDENTITYROUTE, login.Username);

                    _context.UserAuths.Update(authUser);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogCritical("Route: {method}, User: {username} | An internal error occurred: {exception}",
                                     Constants.LOGINIDENTITYROUTE, login.Username, ex.Message);
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

        public async Task<RegisterResponseDTO> Register(RegisterRequestDTO register)
        {
            _logger.LogInformation("Route: {method}, User: {username} | Checking whether user exists",
                                   Constants.REGISTERIDENTITYROUTE, register.Username);

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
                                       Constants.REGISTERIDENTITYROUTE, register.Username);

                return new RegisterResponseDTO { Error = "The given account could not be registered." , Result = false};
            }

            //If the user does not exist, add the user with the given data to Login

            _logger.LogInformation("Route: {method}, User: {username} |  Add the user to Login",
                                   Constants.REGISTERIDENTITYROUTE, register.Username);

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
                                        Constants.REGISTERIDENTITYROUTE, register.Username, ex.Message);
                    throw;
                }
            }

            _logger.LogInformation("Route: {method}, User: {username} |  Add the user to User",
                                   Constants.REGISTERIDENTITYROUTE, register.Username);

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
                                    Constants.REGISTERIDENTITYROUTE, register.Username, ex.Message);
                throw;
            }

            _logger.LogInformation("Route: {method}, User: {username} |  User has been registered",
                                   Constants.REGISTERIDENTITYROUTE, register.Username);

            return new RegisterResponseDTO { Error = string.Empty, Result = true };
        }
    }
}
