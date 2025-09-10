using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CurrencyConversion.API.Models;
using CurrencyConversion.Domain.Entities;
using CurrencyConversion.Service.Rates;
using CurrencyConversion.Service.Repository.IRepository;
using log4net;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static CurrencyConversion.Utility.Enums;

namespace CurrencyConversion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly log4net.ILog _log;

        public AuthController(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _log = LogManager.GetLogger(typeof(AuthController));
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] Models.RegisterRequest request)
        {
            try
            {
                if (await _unitOfWork.Users.Get(u => u.Username == request.Username) != null)
                    return BadRequest("Username already exists");

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var newUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    Username = request.Username,
                    PasswordHash = hashedPassword,
                    Role = request.Role
                };

                _unitOfWork.Users.Add(newUser);
                _unitOfWork.Save();

                return Ok("User registered successfully");
            }
            catch(Exception ex)
            {
                _log.Error(ex);
                return BadRequest("Something went wrong!");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Models.LoginRequest request)
        {
            try
            {
                var user = await _unitOfWork.Users.Get(u => u.Username == request.Username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return Unauthorized("Invalid credentials");

                var token = GenerateJwtToken(user);

                return Ok(new { Bearer = token });
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return BadRequest("Something went wrong!");
            }
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
