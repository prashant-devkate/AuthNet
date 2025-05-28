using AuthNet.Helpers;
using AuthNet.Models;
using AuthNet.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtHelper _jwtHelper;

        public AuthController(UserService userService, JwtHelper jwtHelper)
        {
            _userService = userService;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var hashedPwd = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User { Username = dto.Username, PasswordHash = hashedPwd, Role = dto.Role };

            var success = _userService.Register(user);
            return success ? Ok("User registered successfully") : BadRequest("User already exists");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _userService.Authenticate(dto.Username, dto.Password);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = _jwtHelper.GenerateToken(user.Username, user.Role);
            return Ok(new { token });
        }

    }
}
