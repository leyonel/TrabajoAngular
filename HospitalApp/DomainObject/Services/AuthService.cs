using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DomainObject.Interfaces;
using Models.Models;
using System.Security.Cryptography;
using DataAccess.Interfaces;
using Models.DTOs;

namespace DomainObject.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly TokenValidationParameters _tokenValidationParams;
        private readonly IRefreshToken _refreshToken;

        public AuthService(IConfiguration config, TokenValidationParameters tokenValidationParams, 
            IRefreshToken refreshToken)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"]));
            _tokenValidationParams = tokenValidationParams;
            _refreshToken = refreshToken;
        }
        
        public AuthResult GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Roles)
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                IsUsed = false,
                IsRevoked = false,
                UserID = user.UserID,
                AddedDate = DateTime.UtcNow,
                ExpireDate = DateTime.UtcNow.AddMinutes(7),
                Token = GenerateRefreshToken()
            };

            _refreshToken.AddRefreshToken(refreshToken);

            return new AuthResult()
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                Success = true
            };
        }

        public AuthResult VerifyAndGenerateToken(TokenRequestDto tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Validation 1 - Validation JWT token format:
                _tokenValidationParams.ValidateLifetime = false;

                var accessTokenVerification = jwtTokenHandler
                    .ValidateToken(tokenRequest.Token, _tokenValidationParams, out var validatedToken);


                // Validation 2 - Validate encryption alg:

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg
                        .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

                    if (result == false) return null;
                }

                // Validation 3 - validate expiry date:

                var utcExpiryDate = long
                    .Parse(accessTokenVerification.Claims
                    .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)!
                    .Value);

                var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);

                if (expiryDate > DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has not yet expired"
                        }
                    };
                }

                // validation 4 - validate existence of the token: 

                var storedToken = _refreshToken.FirstOrDefault(tokenRequest.RefreshToken);

                if (storedToken.Token == null)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token does not exist"
                        }
                    };
                }

                // Validation 5 - validate if used:

                if (storedToken.IsUsed)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has been used"
                        }
                    };
                }

                // Validation 6 - validate if revoked
                if (storedToken.IsRevoked)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has been revoked"
                        }
                    };
                }

                // Validation 7 - validate stored token expiry date
                if (storedToken.ExpireDate < DateTime.UtcNow)
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Refresh token has expired"
                        }
                    };
                }

                // update current token:

                storedToken.IsUsed = true;
                _refreshToken.UpdateRefreshToken(storedToken);

                var dbUser = _refreshToken.FindById(storedToken.UserID);

                var newToken = GenerateToken(dbUser);

                // Generate a new token:

                _refreshToken.SaveNewToken(dbUser.UserID, newToken.Token);

                return newToken; 
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("Lifetime validation failed. The token is expired."))
                {

                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Token has expired please re-login"
                        }
                    };

                }
                else
                {
                    return new AuthResult()
                    {
                        Success = false,
                        Errors = new List<string>() {
                            "Something went wrong."
                        }
                    };
                }
            }
        }

        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToUniversalTime();

            return dateTimeVal;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}
