using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<SuperAdmin> SelectSuperAdminByUsernameAsync(string username);
    }
}
