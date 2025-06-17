using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace ProjectMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("token")]
        [EnableRateLimiting("TokenPolicy")]
        public ActionResult CreateToken(string username, string password)
        {
            _logger.LogInformation("CreateToken | Login attempt received for user: {Username}", username);

            try
            {
                if (username == "admin" && password == "password")
                {
                    var jwtSettings = _configuration.GetSection("JWT");

                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

                    var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                    var claims = new[]
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                    var token = new JwtSecurityToken(
                        issuer: jwtSettings["Issuer"],
                        audience: jwtSettings["Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddHours(1),
                        signingCredentials: credentials
                        );

                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                    _logger.LogInformation("CreateToken | Token successfully generated for user: {Username} token: {tokenString}", username, tokenString);
                    return Ok(new { Token = tokenString });
                }

                _logger.LogWarning("CreateToken | Invalid login attempt for user: {Username}", username);
                return Unauthorized();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateToken | Error occured while generating token for user: {Username}", username);
                return StatusCode(500, "An error occured while generating the token.");
            }
        }
    }
}
