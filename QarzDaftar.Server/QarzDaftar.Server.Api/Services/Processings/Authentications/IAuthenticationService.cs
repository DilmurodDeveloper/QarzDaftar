namespace QarzDaftar.Server.Api.Services.Processings.Authentications
{
    public interface IAuthenticationService
    {
        ValueTask<string> AuthenticateSuperAdminAsync(string username, string password);
        ValueTask<string> AuthenticateUserAsync(string username, string password);
        ValueTask RegisterUserAsync(
            string fullName,
            string username,
            string email,
            string password,
            string phoneNumber,
            string shopName,
            string address);
        ValueTask UpdateUserAsync(
            Guid id,
            string fullName,
            string username,
            string email,
            string password,
            string phoneNumber,
            string shopName,
            string address);
        ValueTask LogoutUserAsync(Guid userId);
    }
}
