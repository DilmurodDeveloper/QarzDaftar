using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;
using QarzDaftar.Server.Api.Models.Foundations.Users;

namespace QarzDaftar.Server.Api.Services.Processings.Tokens
{
    public interface ITokenProcessingService
    {
        string CreateToken(User user);
        string CreateTokenForSuperAdmin(SuperAdmin superAdmin);
    }
}
