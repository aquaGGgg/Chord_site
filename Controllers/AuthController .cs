using Microsoft.AspNetCore.Mvc;
using Chords_site.Models;
using Chords_site.Services;
using Microsoft.AspNetCore.Identity.Data;
using RefreshRequestModel = Chords_site.Models.RefreshRequest;
using Chords_site.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;



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
        public async Task<IActionResult> Register([FromBody] UserRegistration model, [FromServices] AppDbContext dbContext)
        {
            if (await dbContext.Users.AnyAsync(u => u.Email == model.Email))
                return BadRequest("User already exists.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = passwordHash
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin model, [FromServices] AppDbContext dbContext)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                return Unauthorized("Invalid email or password.");

            var accessToken = _jwtService.GenerateAccessToken(user.Id, "User");
            var refreshToken = _jwtService.GenerateRefreshToken();

            // You may want to store refresh tokens in a database instead
            RefreshTokens[refreshToken] = user.Email;

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
