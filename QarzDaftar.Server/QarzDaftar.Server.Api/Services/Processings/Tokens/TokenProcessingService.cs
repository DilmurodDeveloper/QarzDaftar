using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;
using QarzDaftar.Server.Api.Models.Foundations.Users;

namespace QarzDaftar.Server.Api.Services.Processings.Tokens
{
    public class TokenProcessingService : ITokenProcessingService
    {
        private readonly IConfiguration configuration;

        public TokenProcessingService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CreateToken(User user)
        {
            var secretKey = configuration["JwtSettings:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["JwtSettings:ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateTokenForSuperAdmin(SuperAdmin superAdmin)
        {
            var secretKey = configuration["JwtSettings:SecretKey"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, superAdmin.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, superAdmin.Username),
                new Claim(ClaimTypes.Name, superAdmin.FullName),
                new Claim(ClaimTypes.Role, superAdmin.Role)
            };

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["JwtSettings:ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
