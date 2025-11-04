using CurrencyTracker.Core.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<IdentityUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string email, string password)
        {
            IdentityUser user = new IdentityUser { UserName = email, Email = email };
            IdentityResult result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("User created successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            IdentityUser? user = await _userManager.FindByEmailAsync(email);
            if (user == null || !(await _userManager.CheckPasswordAsync(user, password)))
                return Unauthorized("Invalid credentials");

            string token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            string configKey = _config["Jwt:Key"] ?? string.Empty;

            if (string.IsNullOrEmpty(configKey))
            {
                return configKey;
            }

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(configKey));

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            int expiryMinutes = int.TryParse(_config["ExpiryMinutes"], out var minutes)
                ? minutes
                : Constants.JwtExpiryTimeDefault; 

            JwtSecurityToken token = new(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(expiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
