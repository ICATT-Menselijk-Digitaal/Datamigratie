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

            // Convert secret key to bytes  
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Set issued-at (iat) timestamp  
            // one minute leeway to account for clock differences between machines
            var issuedAt = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds();

            // Create JWT payload  
            var claims = new List<Claim>
           {
               new ("client_id", clientId),
               new("iat", issuedAt.ToString(), ClaimValueTypes.Integer64)
           };

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
