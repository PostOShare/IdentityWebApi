﻿using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace IdentityWebApi.HelperUtility
{
    public class JWTTokenGenerationHelper
    {
        /// <summary>
        /// Creates an access token that is valid for 15 minutes 
        /// </summary>
        /// <param name="username">Username that sends the token request</param>
        /// <returns>The JWT token generated for the username</returns>
        public string GenerateJWTToken(string username)
        {
            var handler = new JwtSecurityTokenHandler();
            var secret = "dkfgnkdfhfghfghjhjkhdfgdbnmbnsdfsdfhjkhjkssdfsgjgjhbnvbnhgjghjdgdfg";
            var key = Encoding.ASCII.GetBytes(secret);

            var keyDescription = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("username", username),
                    new Claim("iat", 
                              DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()) // issued at timestamp
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                                            SecurityAlgorithms.HmacSha256)
            };

            var jwtToken = handler.CreateToken(keyDescription);
            return handler.WriteToken(jwtToken);
        }
    }
}
