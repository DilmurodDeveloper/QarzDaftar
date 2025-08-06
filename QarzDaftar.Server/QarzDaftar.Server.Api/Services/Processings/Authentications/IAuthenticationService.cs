namespace QarzDaftar.Server.Api.Services.Processings.Authentications
{
    public interface IAuthenticationService
    {
        ValueTask<string> AuthenticateUserAsync(string username, string password);
        ValueTask RegisterUserAsync(string fullName, string username, string email, string password);
        ValueTask LogoutUserAsync(Guid userId);
    }
}
