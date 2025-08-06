using QarzDaftar.Server.Api.Models.Foundations.Users;

namespace QarzDaftar.Server.Api.Services.Processings.Tokens
{
    public interface ITokenProcessingService
    {
        string CreateToken(User user);
    }
}
