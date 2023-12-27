using EntityORM.DatabaseEntity;
using IdentityWebApi.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IdentityWebApi.Controllers
{
    [Route("api/v1/auth/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IdentityPmContext _context;

        public IdentityController(IdentityPmContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("LoginIdentity")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request");
            }
            
            //Check whether a user with username and password is existing
            var current = _context.Logins.Where(user => user.Username.Equals(login.Username) &&
                                                     user.PasswordHash.Equals(login.Password));

            if (current.ToArray().Length == 0)
            {
                return BadRequest("Invalid username and/or password");
            }

            //If the user exists, update current login time
            var currentLogin = _context.Logins.Find(current.ToArray()[0].Username);
            currentLogin.LastLoginTime = DateTime.Now;
            _context.Logins.Update(currentLogin);
            _context.SaveChanges();

            return Ok(new AuthResultDTO { Result = true, Token = ""});
        }

        [HttpPost]
        [Route("RegisterIdentity")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO register)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request");
            }

            //Check whether a user with username is existing
            var current = _context.Logins.Where(user => user.Username.Equals(register.Username));

            if (current.ToArray().Length > 0)
            {
                return BadRequest("The given account could not be registered.");
            }

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
