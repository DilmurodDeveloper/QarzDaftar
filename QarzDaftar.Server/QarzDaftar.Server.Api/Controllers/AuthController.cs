using Microsoft.AspNetCore.Mvc;
using QarzDaftar.Server.Api.Services.Processings.Authentications;
using RESTFulSense.Controllers;

namespace QarzDaftar.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : RESTFulController
    {
        private readonly IAuthenticationService authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            await this.authenticationService.RegisterUserAsync(
                request.FullName,
                request.Username,
                request.Email,
                request.Password,
                request.PhoneNumber,
                request.ShopName,
                request.Address);

            return Ok(new { Message = "Ro'yxatdan o'tish muvaffaqiyatli bajarildi." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            string token = await authenticationService.AuthenticateUserAsync(
                request.Username,
                request.Password);

            return Ok(new { AccessToken = token });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            await authenticationService.LogoutUserAsync(request.UserId);
            return Ok(new { Message = "Logout muvaffaqiyatli bajarildi." });
        }
    }
    public class RegisterRequest
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string ShopName { get; set; }
        public string Address { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class LogoutRequest
    {
        public Guid UserId { get; set; }
    }
}
