using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Chords_site.Services
{
    public class JwtService
    {
        private readonly string _secret;
        private readonly string _refreshSecret;
        private readonly int _accessTokenExpirationMinutes;
        private readonly int _refreshTokenExpirationDays;

        public JwtService(string secret, string refreshSecret, int accessTokenExpirationMinutes, int refreshTokenExpirationDays)
        {
            // Проверка длины секретов для HS256 (минимум 256 бит = 32 байта)
            _secret = EnsureValidSecret(secret, nameof(secret));
            _refreshSecret = EnsureValidSecret(refreshSecret, nameof(refreshSecret));
            _accessTokenExpirationMinutes = accessTokenExpirationMinutes;
            _refreshTokenExpirationDays = refreshTokenExpirationDays;
        }

        private string EnsureValidSecret(string key, string keyName)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Length < 32)
            {
                Console.WriteLine($"{keyName} is invalid or too short. Generating a secure key...");
                return GenerateSecureSecretKey();
            }
            return key;
        }

        public string GenerateAccessToken(Guid userId, string role)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()), // Уникальный идентификатор пользователя
                new Claim(ClaimTypes.Role, role), // Роль пользователя
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Идентификатор токена
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        public static string GenerateSecureSecretKey(int length = 32)
        {
            var randomBytes = new byte[length];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
    }
}
