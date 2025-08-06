using Microsoft.AspNetCore.Identity;
using QarzDaftar.Server.Api.Models.Foundations.Users;
using QarzDaftar.Server.Api.Services.Foundations.Users;
using QarzDaftar.Server.Api.Services.Processings.Tokens;

namespace QarzDaftar.Server.Api.Services.Processings.Authentications
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService userService;
        private readonly ITokenProcessingService tokenProcessingService;
        private readonly IPasswordHasher<User> passwordHasher;

        public AuthenticationService(
            IUserService userService,
            ITokenProcessingService tokenProcessingService,
            IPasswordHasher<User> passwordHasher)
        {
            this.userService = userService;
            this.tokenProcessingService = tokenProcessingService;
            this.passwordHasher = passwordHasher;
        }

        public async ValueTask<string> AuthenticateUserAsync(string username, string password)
        {
            User user = await this.userService.RetrieveUserByUsernameAsync(username);

            if (user is null || user.IsBlocked)
                throw new UnauthorizedAccessException("Username yoki parol noto‘g‘ri.");

            var result = this.passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Username yoki parol noto‘g‘ri.");

            return this.tokenProcessingService.CreateToken(user);
        }
    }
}
