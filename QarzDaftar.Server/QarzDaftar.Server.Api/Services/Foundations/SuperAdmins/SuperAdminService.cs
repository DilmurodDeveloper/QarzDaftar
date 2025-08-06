using Microsoft.Data.SqlClient;
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

                SuperAdmin maybeSuperAdmin =
                    await this.storageBroker.SelectSuperAdminByUsernameAsync(username);

                ValidateStorageSuperAdmin(maybeSuperAdmin, username);

                return maybeSuperAdmin;
            }
            catch (InvalidSuperAdminException invalidSuperAdminException)
            {
                var superAdminValidationException =
                    new SuperAdminValidationException(invalidSuperAdminException);

                this.loggingBroker.LogError(superAdminValidationException);

                throw superAdminValidationException;
            }
            catch (NotFoundSuperAdminException notFoundSuperAdminException)
            {
                var superAdminValidationException =
                    new SuperAdminValidationException(notFoundSuperAdminException);

                this.loggingBroker.LogError(superAdminValidationException);

                throw superAdminValidationException;
            }
            catch (SqlException sqlException)
            {
                var failedStorageException =
                    new FailedSuperAdminStorageException(sqlException);

                var superAdminDependencyException =
                    new SuperAdminDependencyException(failedStorageException);

                this.loggingBroker.LogCritical(superAdminDependencyException);

                throw superAdminDependencyException;
            }
        }
    }
}
