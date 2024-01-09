using IdentityWebApi.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace IdentityWebApi.HelperUtility
{
    public class JWTTokenGenerationHelper
    {
        public string GenerateJWTToken(LoginRequestDTO login)
        {
            var handler = new JwtSecurityTokenHandler();
            var secret = "dkfgnkdfhfghfghjhjkhdfgdbnmbnsdfsdfhjkhjkssdfsgjgjhbnvbnhgjghjdgdfg";
            var key = Encoding.ASCII.GetBytes(secret);

            var keyDescription = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("username", login.Username),
                    new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                                            SecurityAlgorithms.HmacSha256)
            };

            var jwtToken = handler.CreateToken(keyDescription);
            return handler.WriteToken(jwtToken);
        }
    }
}
