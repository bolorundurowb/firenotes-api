using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using firenotes_api.Models.Data;
using Microsoft.IdentityModel.Tokens;

namespace firenotes_api.Configuration
{
    public static class Helpers
    {
        public static string GetToken(User user, int duration = 24, TokenType type = TokenType.Auth)
        {
            Claim[] claims;

            if (type == TokenType.Auth)
            {
                claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("id", user.Id.ToString()) 
                };
            }
            else
            {
                claims = new[]
                {
                    new Claim("email", user.Email)
                };
            }

            var token = new JwtSecurityToken
            (
                Config.Issuer,
                Config.Audience,
                claims,
                expires: DateTime.UtcNow.AddHours(duration),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.Secret)),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        internal static int GetAuthUserData(ClaimsPrincipal claimsPrincipal)
        {
            if (!(claimsPrincipal.Identity is ClaimsIdentity identity))
            {
                throw new Exception("Usre identity could not be retrieved.");
            }

            var id = identity.Claims.First(x => x.Type == "id").Value;
            return int.Parse(id);
        }
        
        internal static string GetResetTokenUserData(string token)
        {
            var securityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = securityTokenHandler.ReadJwtToken(token);
            return jwtSecurityToken?.Claims?.FirstOrDefault(x => x.Type == "email")?.Value;
        }
    }

    public enum TokenType
    {
        Undefined = 0,
        Auth = 1,
        Reset = 2
    }
}