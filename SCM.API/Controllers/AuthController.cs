using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SCM.API.Data;
using SCM.API.DTOs;
using SCM.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SCM_System.DTOs.Auth;
using SCM_System.Helpers;


namespace SCM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Email == dto.Email &&
                u.PasswordHash == PasswordHelper.HashPassword(dto.Password));

            if (user == null)
                return Unauthorized("Invalid credentials");

            
            // For now plain comparison (we will hash later)
            //if (user.PasswordHash != dto.Password)
            //    return Unauthorized("Invalid credentials");

            

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.RoleId.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(_configuration["JwtSettings:ExpiryMinutes"])),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });

        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email and Password required");

            var existingUser = _context.Users
                .Any(u => u.Email == dto.Email);

            if (existingUser)
                return BadRequest("User already exists");

            // Validate role
            var validRoles = new[] { 1, 2, 3, 4 };

            if (!validRoles.Contains(dto.RoleId))
                return BadRequest("Invalid role");

            var hashedPassword = PasswordHelper.HashPassword(dto.Password);

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = hashedPassword,
                RoleId = dto.RoleId
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User registered successfully");
        }
    }
}