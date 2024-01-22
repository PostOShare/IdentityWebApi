using IdentityWebApi.Models;
using System.Security.Cryptography;

namespace IdentityWebApi.HelperUtility
{
    public class RefreshTokenGenerationHelper
    {
        /// <summary>
        /// Creates a refresh token that is valid for 1 day 
        /// </summary>
        /// <returns>The <see cref="Models.TokenModel"/> object that contains the token data</returns>
        public TokenModel GenerateRefreshToken()
        {
            var refresh = new TokenModel
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
                Created = DateTime.Now
            };

            return refresh;
        }
    }
}
