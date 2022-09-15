using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Server.Controllers;

[ApiController]
[Route("[controller]")]
public class JWTAuthController : ControllerBase
{
    private readonly ILogger<JWTAuthController> _logger;
    private readonly IConfiguration _config;
    public JWTAuthController(IConfiguration config, ILogger<JWTAuthController> logger)
    {
        _logger = logger;
        _config = config;
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginModel login)
    {
        var user = AuthenticateUser(login);

        if (user != null)
        {
            var tokenString = GenerateJWT(user);
            return Ok(new { token = tokenString });
        }
        else
        {
            return Unauthorized();
        }
    }

    private UserLoginModel? AuthenticateUser(LoginModel login)
    {
        UserLoginModel? user = null;

        if (login.EmailAddress == "test@gmail.com")
        {
            user = new UserLoginModel { Username = "test", EmailAddress = "test@gmail.com" };
        }
        return user;
    }

    private string GenerateJWT(UserLoginModel userInfo)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtAuth:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


        //claim is used to add identity to JWT token
        var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.EmailAddress),
                new Claim("Date", DateTime.Now.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


        var token = new JwtSecurityToken(_config["JwtAuth:Issuer"],
          _config["JwtAuth:Issuer"],
          claims,
          expires: DateTime.Now.AddMinutes(120),
          signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginModel
{
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string EmailAddress { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
}

public class UserLoginModel
{
    public string Username { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
}