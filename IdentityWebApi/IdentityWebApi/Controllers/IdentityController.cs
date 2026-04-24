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
        public IConfiguration _configuration;

        public IdentityController(IIdentityService identityService, IConfiguration configuration)
        {
            _identityService = identityService;
            _configuration = configuration;
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
        public async Task<IActionResult> Login([FromBody, Required] LoginRequestDTO loginRequestDTO)
        {
            try
            {
                var response = await _identityService.Login(loginRequestDTO);

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
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            try
            {
                var response = await _identityService.Register(registerRequestDTO);

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
        public async Task<IActionResult> GenerateAccessToken([FromBody] CreateTokenRequestDTO createTokenRequestDTO)
        {
            if (string.IsNullOrEmpty(createTokenRequestDTO.RefreshToken))
                return BadRequest(Constants.InvalidRefreshTokenError);

            try
            {
                var response = await _identityService.GenerateAccessToken(createTokenRequestDTO);

                if (!response.Result)
                    return BadRequest(response.Error);
                else
                    return StatusCode(StatusCodes.Status201Created,
                              new AuthResultDTO
                              {
                                  RefreshToken = response.RefreshToken,
                                  AccessToken = response.AccessToken,
                                  Result = true
                              });
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
        public async Task<IActionResult> ValidateAccessToken([FromBody] CreateTokenRequestDTO createTokenRequestDTO)
        {
            if (string.IsNullOrEmpty(createTokenRequestDTO.AccessToken))
                return BadRequest(Constants.InvalidAccessTokenError);

            try
            {
                var key = _configuration.GetSection(Constants.SecretKey).Value;
                var response = await _identityService.ValidateAccessToken(createTokenRequestDTO, key!);

                if (!response.Result)
                    return BadRequest(response.Error);
                else
                    return StatusCode(StatusCodes.Status200OK,
                              new AuthResultDTO
                              {
                                  RefreshToken = response.RefreshToken,
                                  AccessToken = response.AccessToken,
                                  Result = true
                              });
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
    }
}