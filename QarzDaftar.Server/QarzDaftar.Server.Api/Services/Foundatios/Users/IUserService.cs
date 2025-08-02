using QarzDaftar.Server.Api.Models.Foundations.Users;

namespace QarzDaftar.Server.Api.Services.Foundatios.Users
{
    public interface IUserService
    {
        ValueTask<User> AddUserAsync(User user);
        IQueryable<User> RetrieveAllUsers();
    }
}
