using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;

namespace QarzDaftar.Server.Api.Services.Foundations.SuperAdmins
{
    public partial class SuperAdminService : ISuperAdminService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;

        public SuperAdminService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<SuperAdmin> RetrieveSuperAdminByUsernameAsync(string username) =>
        TryCatch(async () =>
        {
            ValidateSuperAdminUsername(username);

            SuperAdmin maybeSuperAdmin =
                await this.storageBroker.SelectSuperAdminByUsernameAsync(username);

            ValidateStorageSuperAdmin(maybeSuperAdmin, username);

            return maybeSuperAdmin;
        });
    }
}
