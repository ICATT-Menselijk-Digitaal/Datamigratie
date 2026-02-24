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
            var timeNow = DateTime.UtcNow.AddMinutes(-1);

            var claims = new Dictionary<string, object>
            {
                { "client_id", clientId },
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                IssuedAt = timeNow,
                // nbf and expires required or else it is automatically added by the library with a default value
                // this keeps time consistent and adds our leeway
                NotBefore = timeNow,
                Expires = timeNow.AddDays(1),
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
