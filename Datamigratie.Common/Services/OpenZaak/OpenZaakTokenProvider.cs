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
            var issuedAt = DateTime.UtcNow;
            
            var claims = new Dictionary<string, object>
            {
                { "client_id", clientId },
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                IssuedAt = issuedAt,
                NotBefore = issuedAt,
                Issuer = clientId,
                Claims = claims,
                Subject = new ClaimsIdentity(),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
