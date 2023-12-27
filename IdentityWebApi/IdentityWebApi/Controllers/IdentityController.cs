using EntityORM.DatabaseEntity;
using IdentityWebApi.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace IdentityWebApi.Controllers
{
    [Route("api/v1/auth/")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IdentityPmContext _context;

        public IdentityController(IdentityPmContext context)
        {
            _context = context;
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
        [SwaggerResponse((int) HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(BadRequest))]
        public async Task<IActionResult> Login([FromBody, Required] LoginRequestDTO login)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request");
            
            //Check whether a user with username and password is existing
            var current = await _context.Logins.Where(
                                                        user => user.Username.Equals(login.Username) &&
                                                        user.PasswordHash.Equals(login.Password)
                                                    ).FirstOrDefaultAsync();

            if (current == null)
                return BadRequest("Invalid username and/or password");

            //If the user exists, update current login time
            current.LastLoginTime = DateTime.Now;
            _context.Logins.Update(current);
            _context.SaveChanges();

            return Ok(new AuthResultDTO { Result = true, Token = ""});
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

            //Check whether a user with username is existing
            var current = await _context.Logins.Where(user => user.Username.Equals(register.Username))
                                               .FirstOrDefaultAsync();

            if (current == null)
                return BadRequest("The given account could not be registered.");

            //If the user does not exist, add user with given data
            var login = new Login
            {
                Username = register.Username,
                PasswordHash = register.Password,
                RegisteredDate = DateTime.Now,
                LastLoginTime = DateTime.Now,
                UserRole = register.UserRole,
                IsActive = true
            };

            _context.Add(login);
            _context.SaveChanges();

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

            _context.Add(user);
            _context.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
