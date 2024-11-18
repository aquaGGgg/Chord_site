using Microsoft.AspNetCore.Mvc;
using Chords_site.Models;
using Chords_site.Services;
using Microsoft.AspNetCore.Identity.Data;
using RefreshRequestModel = Chords_site.Models.RefreshRequest;

namespace Chords_site.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        // Replace this with a database or repository in a real application
        private static readonly Dictionary<string, string> Users = new(); // Email -> PasswordHash
        private static readonly Dictionary<string, string> RefreshTokens = new(); // RefreshToken -> Email

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegistration model)
        {
            if (Users.ContainsKey(model.Email))
                return BadRequest("User already exists.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            Users[model.Email] = passwordHash;

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin model)
        {
            if (!Users.ContainsKey(model.Email) || !BCrypt.Net.BCrypt.Verify(model.Password, Users[model.Email]))
                return Unauthorized("Invalid email or password.");

            var userId = Guid.NewGuid(); // Replace with real user ID
            var accessToken = _jwtService.GenerateAccessToken(userId, "User");
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Save the refresh token
            RefreshTokens[refreshToken] = model.Email;

            return Ok(new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("refresh")]

        public IActionResult Refresh([FromBody] RefreshRequestModel model)
        {

            if (string.IsNullOrWhiteSpace(model.RefreshToken) || !RefreshTokens.ContainsKey(model.RefreshToken))
                return Unauthorized("Invalid or missing refresh token.");

            var email = RefreshTokens[model.RefreshToken];
            var userId = Guid.NewGuid(); // Replace with real user ID

            var newAccessToken = _jwtService.GenerateAccessToken(userId, "User");
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Update the refresh token
            RefreshTokens.Remove(model.RefreshToken);
            RefreshTokens[newRefreshToken] = email;

            return Ok(new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
    }
}
