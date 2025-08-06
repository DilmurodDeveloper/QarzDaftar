using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins.Exceptions;

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

        public async ValueTask<SuperAdmin> RetrieveSuperAdminByUsernameAsync(string username)
        {
            try
            {
                ValidateSuperAdminUsername(username);

                return await this.storageBroker.SelectSuperAdminByUsernameAsync(username);
            }
            catch (InvalidSuperAdminException invalidSuperAdminException)
            {
                var superAdminValidationException =
                    new SuperAdminValidationException(invalidSuperAdminException);

                this.loggingBroker.LogError(superAdminValidationException);

                throw superAdminValidationException;
            }
        }
    }
}
