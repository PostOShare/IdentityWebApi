using IdentityWebApi.Services;
using IdentityWebApiCommon.HelperUtility;
using IdentityWebApiCommon.Models.DTO.Request;
using IdentityWebApiCommon.Models.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace IdentityWebApi.Controllers
{
    [Route("api/v1/auth/")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        public IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }


        /// <summary> 
        /// Checks whether a username/password exists
        /// </summary>
        /// <returns> 
        /// A ObjectResult whether the user exists (Status OK), Not found or
        /// data is invalid (Status BadRequest), or an internal error occurred 
        /// (Status InternalServerError)
        /// </returns>
        [HttpPost]
        [Route("login-identity")]
        [SwaggerOperation("Checks whether a username/password exists")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody, Required] LoginRequestDTO loginRequest)
        {
            try
            {
                var response = await _identityService.Login(loginRequest);

                if(response.Error.Equals(Constants.UserValidationError))
                    return BadRequest(Constants.UserValidationError);
                else
                    return Ok(response);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new AuthResultDTO
                                  {
                                      Error = ex.Message,
                                      Result = false
                                  });
            }
        }

        /// <summary> 
        /// Registers data of a user
        /// </summary>
        /// <returns> 
        /// A ObjectResult whether the user was created (Status Created), User exists or
        /// data is invalid (Status BadRequest), or an internal error occurred 
        /// (Status InternalServerError) 
        /// </returns>
        [HttpPost]
        [Route("register-identity")]
        [SwaggerOperation("Registers the user data with given username")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequest)
        {
            try
            {
                var response = await _identityService.Register(registerRequest);

                if (!response.Result)
                    return BadRequest(Constants.UserExistsError);
                else
                    return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new AuthResultDTO
                                  {
                                      Error = ex.Message,
                                      Result = false
                                  });
            }
        }

        
        /// <summary> 
        /// Checks whether a username and email address exists
        /// </summary>
        /// <returns> 
        /// A ObjectResult whether the user exists (Status OK),
        /// username was not found or data is invalid (Status Bad Request),
        /// or an internal error occurred (Status InternalServerError)
        /// </returns>
        [HttpPost]
        [Route("search-identity")]
        [SwaggerOperation("Validate if the username and email exists")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UserData([FromBody, Required] UpdateRequestDTO updateRequestDTO)
        {
            try
            {
                var response = await _identityService.UserData(updateRequestDTO);

                if (!response.Result)
                    return BadRequest(response.Error);
                else
                    return Ok(new AuthResultDTO { Result = true });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new AuthResultDTO
                                  {
                                      Error = ex.Message,
                                      Result = false
                                  });
            }
        }


        /// <summary> 
        /// Generates an OTP, saves the OTP to DB and sends the OTP to the user's email
        /// </summary>
        /// <returns> 
        /// A ObjectResult whether the OTP was sent (Status Created),
        /// username was not found or data is invalid (Status Bad Request)
        /// or email was not sent (Status Internal Server Error)
        /// </returns>
        [HttpPost]
        [Route("verify-identity")]
        [SwaggerOperation("Check username and send OTP")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SendVerification([FromBody] UpdateRequestDTO updateRequestDTO)
        {
            try
            {
                var response = await _identityService.SendVerification(updateRequestDTO);

                if (!response.Result)
                    return BadRequest(response.Error);
                else
                    return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new AuthResultDTO
                                  {
                                      Error = ex.Message,
                                      Result = false
                                  });
            }
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
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ValidatePasscode([FromBody] UpdateRequestDTO updateRequestDTO)
        {
            try
            {
                var response = await _identityService.ValidatePasscode(updateRequestDTO);

                if (!response.Result)
                    return BadRequest(response.Error);
                else
                    return Ok(new AuthResultDTO { Result = true });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new AuthResultDTO
                                  {
                                      Error = ex.Message,
                                      Result = false
                                  });
            }
        }

        /// <summary> 
        /// Checks the username and updates the Key and Salt for the username
        /// </summary>
        /// <returns> 
        /// A ObjectResult whether the Key and Salt were updated (Status OK), 
        /// Not updated (Status Internal Server Error) or
        /// data is invalid (Status Bad Request)
        /// </returns>
        [HttpPatch]
        [Route("change-credentials-identity")]
        [SwaggerOperation("Updates key and salt based on password sent in request for a username")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateKeySalt([FromBody, Required] UpdateRequestDTO updateRequestDTO)
        {
            try
            {
                var response = await _identityService.UpdateKeySalt(updateRequestDTO);

                if (!response.Result)
                    return BadRequest(response.Error);
                else
                    return Ok(new AuthResultDTO { Result = true });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new AuthResultDTO
                                  {
                                      Error = ex.Message,
                                      Result = false
                                  });
            }
        }

        /*
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
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GenerateAccessToken([FromBody] CreateTokenRequestDTO request)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest("Invalid request");

            _logger.LogInformation("Route: {method}, Refresh token: {token} | Checking whether the refresh token exists",
                                   Constants.GENERATEACCESSTOKENIdentityRoute, request.RefreshToken);

            UserAuth? authUser = null;
            try
            {
                authUser = await _context.UserAuths.Where(user => user.Token.Equals(request.RefreshToken))
                                                       .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Exception while querying SQL database {exception}", ex.Message);

                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new AuthResultDTO
                                  {
                                      Error = "An internal error occurred",
                                      Result = false
                                  });
            }

            if (authUser == null)
            {
                _logger.LogError("Route: {method}, Refresh token: {token} | Invalid refresh token",
                                 Constants.GENERATEACCESSTOKENIdentityRoute, request.RefreshToken);

                return BadRequest("Invalid request");
            }
                

            var refresh = authUser.Token;

            // if the refresh token's created time is more than 1 day it is expired
            if (authUser.CreatedTime.AddDays(1) > DateTime.UtcNow)
            {
                _logger.LogInformation("Route: {method}, Refresh token: {token} | Generating refresh token",
                                  Constants.GENERATEACCESSTOKENIdentityRoute, request.RefreshToken);

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

                    return StatusCode(StatusCodes.Status500InternalServerError,
                                      new AuthResultDTO
                                      {
                                          Error = "An internal error occurred",
                                          Result = false
                                      });
                }
            }

            _logger.LogInformation("Route: {method}, Refresh token: {token} | Generating access token",
                                  Constants.GENERATEACCESSTOKENIdentityRoute, request.RefreshToken);

            var access = new JWTTokenGenerationHelper().GenerateJWTToken(authUser.Username);

            _logger.LogInformation("Route: {method}, Refresh token: {token} | Token(s) were created",
                                  Constants.GENERATEACCESSTOKENIdentityRoute, request.RefreshToken);

            return StatusCode(StatusCodes.Status201Created,
                              new AuthResultDTO
                              {
                                  RefreshToken = refresh,
                                  AccessToken = access,
                                  Result = true
                              });
        }

        /// <summary> Validates an access token
        /// </summary>
        /// <returns> 
        /// A ActionResult whether the token is valid (Status Ok),
        /// not valid or data is invalid (Status Bad Request)
        /// </returns>
        [HttpPost]
        [Route("validate-accessToken")]
        [SwaggerOperation("Validates an access token")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        public ActionResult ValidateAccessToken([FromBody] CreateTokenRequestDTO request)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(request.AccessToken))
                return BadRequest("Invalid request");

            _logger.LogInformation("Route: {method}, Access token: {token} | Validating the access token",
                                   Constants.VALIDATEACCESSTOKENIdentityRoute, request.AccessToken);

            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("dkfgnkdfhfghfghjhjkhdfgdbnmbnsdfsdfhjkhjkssdfsgjgjhbnvbnhgjghjdgdfg");

            JwtSecurityToken? token = null;
            try
            {
                handler.ValidateToken(request.AccessToken, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = false
                }, out var validateToken);

                token = (JwtSecurityToken)validateToken;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("The token is invalid or unknown exception: {exception}", ex.Message);

                return StatusCode(StatusCodes.Status400BadRequest,
                                  new AuthResultDTO
                                  {
                                      Error = "The token is invalid",
                                      Result = false
                                  });
            }

            
            var expiry = Convert.ToInt64(token.Claims.Where(p => p.Type == "exp").FirstOrDefault()?.Value);
            var expired = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() > expiry;

            if (expired)
            {
                _logger.LogInformation("Route: {method}, Access token: {token} | The access token is valid",
                                   Constants.VALIDATEACCESSTOKENIdentityRoute, request.AccessToken);

                return BadRequest("Token is expired.");
            }               
            else
            {
                _logger.LogInformation("Route: {method}, Access token: {token} | The access token is expired",
                                   Constants.VALIDATEACCESSTOKENIdentityRoute, request.AccessToken);

                return Ok();
            }                          
        }
        */
    }
}