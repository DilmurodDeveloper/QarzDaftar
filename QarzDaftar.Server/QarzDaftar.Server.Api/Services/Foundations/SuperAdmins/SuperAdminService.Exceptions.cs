using Microsoft.Data.SqlClient;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins.Exceptions;
using Xeptions;

namespace QarzDaftar.Server.Api.Services.Foundations.SuperAdmins
{
    public partial class SuperAdminService
    {
        private delegate ValueTask<SuperAdmin> ReturningSuperAdminFunction();

        private async ValueTask<SuperAdmin> TryCatch(ReturningSuperAdminFunction returningSuperAdminFunction)
        {
            try
            {
                return await returningSuperAdminFunction();
            }
            catch (InvalidSuperAdminException invalidSuperAdminException)
            {
                throw CreateAndLogValidationException(invalidSuperAdminException);
            }
            catch (NotFoundSuperAdminException notFoundSuperAdminException)
            {
                throw CreateAndLogValidationException(notFoundSuperAdminException);
            }
            catch (SqlException sqlException)
            {
                var failedSuperAdminStorageException =
                    new FailedSuperAdminStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedSuperAdminStorageException);
            }
            catch (Exception exception)
            {
                var failedSuperAdminServiceException =
                    new FailedSuperAdminServiceException(exception);

                throw CreateAndLogServiceException(failedSuperAdminServiceException);
            }
        }

        private SuperAdminValidationException CreateAndLogValidationException(Xeption exception)
        {
            var SuperAdminValidationException = new SuperAdminValidationException(exception);
            this.loggingBroker.LogError(SuperAdminValidationException);

            return SuperAdminValidationException;
        }

        private SuperAdminDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var SuperAdminDependencyException = new SuperAdminDependencyException(exception);
            this.loggingBroker.LogCritical(SuperAdminDependencyException);

            return SuperAdminDependencyException;
        }

        private SuperAdminServiceException CreateAndLogServiceException(Xeption exception)
        {
            var SuperAdminServiceException = new SuperAdminServiceException(exception);
            this.loggingBroker.LogError(SuperAdminServiceException);

            return SuperAdminServiceException;
        }
    }
}
