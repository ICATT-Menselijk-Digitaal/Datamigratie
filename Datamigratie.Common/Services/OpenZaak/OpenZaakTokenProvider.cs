using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Datamigratie.Common.Services.OpenZaak
{
    public interface IOpenZaakTokenProvider
    {
        string GetToken();
        bool IsTokenExpired();
    }

    internal class OpenZaakTokenProvider : IOpenZaakTokenProvider
    {
        private readonly string _jwtSecretKey;
        private readonly string _clientId;
        private readonly object _lock = new();
        private string? _currentToken;
        private DateTime _tokenExpiresAt;

        // Token lifetime: 1 hour
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromSeconds(30);
        // Refresh buffer: refresh token 5 minutes before expiration
        private static readonly TimeSpan RefreshBuffer = TimeSpan.FromSeconds(1);

        public OpenZaakTokenProvider(string jwtSecretKey, string clientId)
        {
            _jwtSecretKey = jwtSecretKey ?? throw new ArgumentNullException(nameof(jwtSecretKey));
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
        }

        public string GetToken()
        {
            lock (_lock)
            {
                if (_currentToken == null || IsTokenExpired())
                {
                    _currentToken = GenerateZakenApiToken();
                    _tokenExpiresAt = DateTime.UtcNow.Add(TokenLifetime);
                }
                return _currentToken;
            }
        }

        public bool IsTokenExpired()
        {
            return DateTime.UtcNow.Add(RefreshBuffer) >= _tokenExpiresAt;
        }

        private string GenerateZakenApiToken()
        {
            // one minute leeway to account for clock differences between machines
            var issuedAt = DateTime.UtcNow.AddMinutes(-1);
            var issuer = "kissdev";

            var claims = new Dictionary<string, object>
            {
                { "client_id", _clientId },
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddMinutes(1),
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
