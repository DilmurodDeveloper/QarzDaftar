using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions;
using Xeptions;

namespace QarzDaftar.Server.Api.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogService
    {
        private delegate ValueTask<UserPaymentLog> ReturningUserPaymentLogFunction();
        private delegate IQueryable<UserPaymentLog> ReturningUserPaymentLogsFunction();

        private async ValueTask<UserPaymentLog> TryCatch(ReturningUserPaymentLogFunction returningUserPaymentLogFunction)
        {
            try
            {
                return await returningUserPaymentLogFunction();
            }
            catch (NullUserPaymentLogException nullUserPaymentLogException)
            {
                throw CreateAndLogValidationException(nullUserPaymentLogException);
            }
            catch (InvalidUserPaymentLogException invalidUserPaymentLogException)
            {
                throw CreateAndLogValidationException(invalidUserPaymentLogException);
            }
            catch (NotFoundUserPaymentLogException notFoundUserPaymentLogException)
            {
                throw CreateAndLogValidationException(notFoundUserPaymentLogException);
            }
            catch (SqlException sqlException)
            {
                var failedUserPaymentLogStorageException =
                    new FailedUserPaymentLogStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedUserPaymentLogStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsUserPaymentLogException =
                    new AlreadyExistsUserPaymentLogException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsUserPaymentLogException);
            }
            catch (Exception exception)
            {
                var failedUserPaymentLogServiceException =
                    new FailedUserPaymentLogServiceException(exception);

                throw CreateAndLogServiceException(failedUserPaymentLogServiceException);
            }
        }

        private IQueryable<UserPaymentLog> TryCatch(ReturningUserPaymentLogsFunction returningUserPaymentLogsFunction)
        {
            try
            {
                return returningUserPaymentLogsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedUserPaymentLogStorageException =
                    new FailedUserPaymentLogStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedUserPaymentLogStorageException);
            }
            catch (Exception exception)
            {
                var failedUserPaymentLogServiceException =
                    new FailedUserPaymentLogServiceException(exception);

                throw CreateAndLogServiceException(failedUserPaymentLogServiceException);
            }
        }

        private UserPaymentLogValidationException CreateAndLogValidationException(Xeption exception)
        {
            var UserPaymentLogValidationException = new UserPaymentLogValidationException(exception);
            this.loggingBroker.LogError(UserPaymentLogValidationException);

            return UserPaymentLogValidationException;
        }

        private UserPaymentLogDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var UserPaymentLogDependencyException = new UserPaymentLogDependencyException(exception);
            this.loggingBroker.LogCritical(UserPaymentLogDependencyException);

            return UserPaymentLogDependencyException;
        }

        private UserPaymentLogDependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var UserPaymentLogDependencyValidationException =
                new UserPaymentLogDependencyValidationException(exception);

            this.loggingBroker.LogError(UserPaymentLogDependencyValidationException);

            return UserPaymentLogDependencyValidationException;
        }

        private UserPaymentLogServiceException CreateAndLogServiceException(Xeption exception)
        {
            var UserPaymentLogServiceException = new UserPaymentLogServiceException(exception);
            this.loggingBroker.LogError(UserPaymentLogServiceException);

            return UserPaymentLogServiceException;
        }
    }
}
