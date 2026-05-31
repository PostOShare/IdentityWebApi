using IdentityWebApi.Services;
using IdentityWebApiCommon.HelperUtility;
using IdentityWebApiCommon.Models.DTO;
using IdentityWebApiCommon.Models.DTO.Request;
using IdentityWebApiCommon.Models.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace IdentityWebApi.Controllers
{
    [Route("api/v1/user/")]
    [ApiController]
    public class PersonController: ControllerBase
    {
        public IUserService _userService;

        public PersonController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("live")]
        public IActionResult Live()
        {
            return Ok("Identity API is live");
        }

        /// <summary> 
        /// Retrieves user information based on username and token
        /// </summary>
        /// <returns> 
        /// A ObjectResult with user information (Status OK), Not found or
        /// data is invalid (Status BadRequest), or an internal error occurred 
        /// (Status InternalServerError)
        /// </returns>
        [HttpPost]
        [Route("list-userdata")]
        [SwaggerOperation("Retrieves user information based on username and token")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ListUserData([FromBody, Required] ListUserDataRequestDTO listUserDataRequest)
        {
            try
            {
                var response = await _userService.ListUserData(listUserDataRequest);

                if (response.Error.Equals(Constants.UsernameTokenError))
                    return BadRequest(Constants.UsernameTokenError);
                else
                    return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new IdentityWebApiCommon.Models.DTO.Response.UserDTO
                                  {
                                      Error = ex.Message,
                                      Result = false
                                  });
            }
        }

        /// <summary> 
        /// Saves/Updates user information based on username and token
        /// </summary>
        /// <returns> 
        /// A ObjectResult whether the user information was successfully saved/Updated (Status Created),
        /// data is invalid (Status BadRequest), or an internal error occurred 
        /// (Status InternalServerError)
        /// </returns>
        [HttpPost]
        [Route("upsert-userdata")]
        [SwaggerOperation("Saves/Updates user information based on username and token")]
        [SwaggerResponse((int)HttpStatusCode.Created)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpsertUserData([FromBody, Required] SaveUserRequestDTO saveUserDataRequest)
        {
            try
            {
                var response = await _userService.UpsertUserData(saveUserDataRequest);

                if (response.Error.Equals(Constants.UsernameTokenError))
                    return BadRequest(Constants.UsernameTokenError);
                else
                    return Created(string.Empty, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                                  new IdentityWebApiCommon.Models.DTO.Response.UserDTO
                                  {
                                      Error = ex.Message,
                                      Result = false
                                  });
            }
        }
    }
}