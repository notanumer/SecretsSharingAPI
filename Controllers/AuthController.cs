using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecretsSharingAPI.EF;
using SecretsSharingAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SecretsSharingAPI.Controllers
{
    /// <summary>
    /// Controller for User Registration and Login
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserContext _users;
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration, UserContext users)
        {
            _users = users;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto dto)
        {
            var currentUser = await _users.Users.FirstOrDefaultAsync(user => user.Email == dto.Email);
            if (currentUser == null)
            {
                return BadRequest("User not found");
            }

            if (!VerifyPasswordHash(dto.Password, currentUser.PasswordHash, currentUser.PasswordSalt))
            {
                return BadRequest("password is wrong");
            }
            var token = CreateToken(currentUser);

            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto dto)
        {
            HashPassword(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var newUser = new User();
            newUser.PasswordHash = passwordHash;
            newUser.PasswordSalt = passwordSalt;
            newUser.Email = dto.Email;
            _users.Users.Add(newUser);
            _users.SaveChanges(true);
            return Ok(newUser);
        }

        private void HashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetSection("AppSetting:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
