using QarzDaftar.Server.Api.Models.Foundations.Users;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<User> InsertUserAsync(User user);
    }
}
