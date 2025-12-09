using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Datamigratie.Common.Services.OpenZaak
{
    internal class OpenZaakTokenProvider
    {
        public static string GenerateZakenApiToken(string jwtSecretKey, string clientId)
        {
            // one minute leeway to account for clock differences between machines
            var issuedAt = DateTime.UtcNow.AddMinutes(-1);
            var issuer = "kissdev";

            var claims = new Dictionary<string, object>
            {
                { "client_id", clientId },
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                IssuedAt = issuedAt,
                Issuer = issuer,
                Claims = claims,
                Subject = new ClaimsIdentity(),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
