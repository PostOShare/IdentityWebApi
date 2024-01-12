using EntityORM.DatabaseEntity;
using IdentityWebApi.HelperUtility;
using IdentityWebApi.Models;
using IdentityWebApi.Models.DTO;
using IdentityWebApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security.Cryptography;

namespace IdentityWebApi.Controllers
{
    [Route("api/v1/auth/")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IdentityPmContext _context;
        private readonly IEmailService _mailService;

        public IdentityController(IdentityPmContext context, IEmailService emailService)
        {
            _context = context;
            _mailService = emailService;
        }


        /// <summary> 
        /// Checks whether a username/password exists
        /// </summary>
        /// <returns> 
        /// A ObjectResult whether the user exists (Status OK), Not found or
        /// data is invalid (Status Bad Request)
        /// </returns>
        [HttpPost]
        [Route("login-identity")]
        [SwaggerOperation("Checks whether a username/password exists")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(BadRequest))]
        public async Task<IActionResult> Login([FromBody, Required] LoginRequestDTO login)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request");

            // Check whether a user with the username and password exists
            var current = await _context.Logins.Where(user => user.Username.Equals(login.Username))
                                               .FirstOrDefaultAsync();

            if (current == null)
                return BadRequest("Invalid username and/or password");

            var exists = false;

            using (var deriveBytes = new Rfc2898DeriveBytes(login.Password, Convert.FromBase64String(current.Salt)))
            {
                byte[] newKey = deriveBytes.GetBytes(20);
                exists = newKey.SequenceEqual(Convert.FromBase64String(current.Key));
            }

            if (!exists)
                return BadRequest("Invalid username and/or password");

            // If the user exists, update login time
            current.LastLoginTime = DateTime.Now;

            try
            {
                _context.Logins.Update(current);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Log exception to Cloud Log (to be implemented)
                return StatusCode(StatusCodes.Status500InternalServerError,
                                new AuthResultDTO
                                {
                                    Error = "An internal error occurred",
                                    Result = false
                                });
            }

            //Generate a refresh token and save in DB
            var refresh = new RefreshTokenGenerationHelper().GenerateRefreshToken().Token;
            var authUser = await _context.UserAuths.Where(user => user.Username.Equals(login.Username))
                                                   .FirstOrDefaultAsync();

            if (authUser == null)
            {
                var userAuth = new UserAuth
                {
                    Username = current.Username,
                    Token = refresh,
                    CreatedTime = DateTime.Now,
                    Enabled = true
                };

                try
                {
                    _context.Add(userAuth);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // Log exception to Cloud Log (to be implemented)
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }
            else
            {
                authUser.Token = refresh;
                authUser.CreatedTime = DateTime.Now;

                try
                {
                    _context.UserAuths.Update(authUser);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // Log exception to Cloud Log (to be implemented)
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }

            }

            return Ok(new AuthResultDTO
            {
                Result = true,
                RefreshToken = refresh,
                AccessToken = new JWTTokenGenerationHelper().GenerateJWTToken(current.Username)
            });
        }

        /// <summary> 
        /// Registers data of a user
        /// </summary>
        /// <returns> 
        /// A ObjectResult whether the user was created (Status Created), Not found or
        /// data is invalid (Status Bad Request)
        /// </returns>
        [HttpPost]
        [Route("register-identity")]
        [SwaggerOperation("Registers the user data with given username")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(BadRequest))]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO register)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request");

            //Check whether a user with given username exists
            var current = await _context.Logins.Where(user => user.Username.Equals(register.Username))
                                               .FirstOrDefaultAsync();

            if (current != null)
                return BadRequest("The given account could not be registered.");

            //If the user does not exist, add the user with given data to login          
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
                    // Log exception to Cloud Log (to be implemented)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                                    new AuthResultDTO
                                    {
                                        Error = "An error occurred when adding user",
                                        Result = false
                                    });
                }
            }

            //If the user does not exist, add the user with given data to user
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
                // Log exception to Cloud Log (to be implemented)
                return StatusCode(StatusCodes.Status500InternalServerError,
                                new AuthResultDTO
                                {
                                    Error = "An error occurred when adding user",
                                    Result = false
                                });
            }

            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary> 
        /// Checks whether a username and email address exists
        /// </summary>
        /// <returns> 
        /// A ObjectResult whether the user exists (Status OK),
        /// username was not found or data is invalid (Status Bad Request)
        /// </returns>
        [HttpPost]
        [Route("search-identity")]
        [SwaggerOperation("Validate if the username and email exists")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(BadRequest))]
        public async Task<IActionResult> UserData([FromBody, Required] UpdateRequestDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request");

            // Check whether a user with the username exists
            var username = await _context.Logins.Where(user => user.Username.Equals(userDTO.Username))
                                               .FirstOrDefaultAsync();
            var email = await _context.Users.Where(user => user.EmailAddress.Equals(userDTO.EmailAddress))
                                               .FirstOrDefaultAsync();

            if (username == null || email == null)
                return BadRequest("Invalid username and/or email");

            return Ok(new AuthResultDTO { Result = true });
        }

        /// <summary> 
        /// Generates an OTP, saves the OTP to DB and sends the OTP to the user's email
        /// </summary>
        /// <returns> 
        /// A ObjectResult whether the OTP was created (Status Created)
        /// or email was not sent (Status Internal Server Error)
        /// </returns>
        [HttpPost]
        [Route("verify-identity")]
        [SwaggerOperation("Check email and send OTP")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        public async Task<IActionResult> SendVerification([FromBody] UpdateRequestDTO userDTO)
        {
            var user = await _context.Otpvalidates.Where(user => user.Username.Equals(userDTO.Username))
                                                  .FirstOrDefaultAsync();

            var otpModel = new OTPModel();
            var otp = otpModel.CreateOTP();

            var otpvalidate = new Otpvalidate
            {
                Username = userDTO.Username,
                Otp = otp,
                RequestedTime = DateTime.Now,
                RetryAttempt = 0
            };

            try
            {
                _context.Otpvalidates.Add(otpvalidate);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Log exception to Cloud Log (to be implemented)
                return StatusCode(StatusCodes.Status500InternalServerError,
                                 new AuthResultDTO
                                 {
                                     Error = "An error occurred when adding user",
                                     Result = false
                                 });
            }

            var subject = "PostOShare OTP";
            var body = otpModel.Body(otp);

            var sent = await _mailService.SendMail(userDTO.EmailAddress, subject, body);

            var code = sent ? StatusCodes.Status201Created : StatusCodes.Status500InternalServerError;
            return StatusCode(code);
        }

        /// <summary> 
        /// Checks if the OTP response from the input is valid
        /// </summary>
        /// <returns> 
        /// A ObjectResult whether the OTP is valid (Status Ok),
        /// OTP details were not updated (Status Internal Server Error), 
        /// or the OTP is invalid (Status Bad Request)
        /// </returns>
        [HttpPost]
        [Route("validate-passcode-identity")]
        [SwaggerOperation("Checks whether OTP response is correct")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(BadRequest))]
        public async Task<IActionResult> ValidatePasscode([FromBody] UpdateRequestDTO userDTO)
        {
            var exist = await _context.Otpvalidates.Where(user => user.Username.Equals(userDTO.Username))
                                                   .FirstOrDefaultAsync();

            if (exist.Otp == userDTO.Otp)
            {
                return StatusCode(StatusCodes.Status200OK);
            }
            else
            {
                if (exist.RetryAttempt < 4)
                {
                    exist.RetryAttempt++;

                    try
                    {
                        _context.Otpvalidates.Update(exist);
                        _context.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        // Log exception to Cloud Log (to be implemented)
                        return StatusCode(StatusCodes.Status500InternalServerError,
                                         new AuthResultDTO
                                         {
                                             Error = "An error occurred when validating the OTP",
                                             Result = false
                                         });
                    }

                    return StatusCode(StatusCodes.Status400BadRequest);
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                                      new AuthResultDTO
                                      {
                                          Error = "Cannot try more than maximum attempts",
                                          Result = false
                                      });
                }
            }
        }

        /// <summary> 
        /// Checks the username and updates the Key and Salt
        /// </summary>
        /// <returns> 
        /// A ObjectResult whether the Key and Salt were updated (Status OK), 
        /// Not updated (Status Not Modified) or
        /// data is invalid (Status Bad Request)
        /// </returns>
        [HttpPatch]
        [Route("change-credentials-identity")]
        [SwaggerOperation("Updates key and salt based on password sent in request for a username")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(BadRequest))]
        public async Task<IActionResult> UpdateKeySalt([FromBody, Required] UpdateRequestDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request");

            // Check whether a user with the username and password exists
            var current = await _context.Logins.Where(user => user.Username.Equals(userDTO.Username))
                                               .FirstOrDefaultAsync();

            using (var deriveBytes = new Rfc2898DeriveBytes(userDTO.Password, 20))
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
                    // Log exception to Cloud Log (to be implemented)
                    return StatusCode(StatusCodes.Status304NotModified,
                                    new AuthResultDTO
                                    {
                                        Error = "An error occurred when updating password",
                                        Result = false
                                    });
                }
            }

            return Ok(new AuthResultDTO { Result = true });
        }

        /// <summary> Creates an access token based on the user's refresh token
        /// </summary>
        /// <returns> 
        /// A ObjectResult whether the token was created (Status Created),
        /// access token was not updated (Status Internal Server Error) or
        /// data is invalid (Status Bad Request)
        /// </returns>
        [HttpPost]
        [Route("generate-accessToken")]
        [SwaggerOperation("Creates an access token")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(BadRequest))]
        public async Task<IActionResult> GenerateAccessToken([FromBody] CreateTokenRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request");

            var authUser = await _context.UserAuths.Where(user => user.Token.Equals(request.RefreshToken))
                                                   .FirstOrDefaultAsync();

            if (authUser == null)
                return BadRequest("Invalid request");

            var refresh = authUser.Token;

            // if the refresh token's created time is more than 1 day it is expired
            if (authUser.CreatedTime.AddDays(1) > DateTime.Now)
            {
                refresh = new RefreshTokenGenerationHelper().GenerateRefreshToken().Token;
                authUser.CreatedTime = DateTime.Now;

                try
                {
                    _context.UserAuths.Update(authUser);
                    _context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // Log exception to Cloud Log (to be implemented)
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            var access = new JWTTokenGenerationHelper().GenerateJWTToken(authUser.Username);

            return StatusCode(StatusCodes.Status201Created,
                              new AuthResultDTO
                              {
                                  RefreshToken = refresh,
                                  AccessToken = access,
                                  Result = true
                              });
        }
    }
}