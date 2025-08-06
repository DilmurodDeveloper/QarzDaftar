namespace QarzDaftar.Server.Api.Services.Processings.Authentications
{
    public interface IAuthenticationService
    {
        ValueTask<string> AuthenticateUserAsync(string username, string password);
    }
}
