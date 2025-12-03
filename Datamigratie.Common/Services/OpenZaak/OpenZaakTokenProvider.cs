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
            var now = DateTimeOffset.UtcNow;
            // one minute leeway to account for clock differences between machines
            var issuedAt = now.AddMinutes(-1);
            var iat = issuedAt.ToUnixTimeSeconds();

            var claims = new Dictionary<string, object>
            {
                { "client_id", clientId },
                { "iat", iat }
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                IssuedAt = issuedAt.DateTime,
                NotBefore = issuedAt.DateTime,
                Claims = claims,
                Subject = new ClaimsIdentity(),
                Expires = now.AddDays(7).DateTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
