using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogService : IUserPaymentLogService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public UserPaymentLogService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public async ValueTask<UserPaymentLog> AddUserPaymentLogAsync(UserPaymentLog userPaymentLog)
        {
            try
            {
                ValidateUserPaymentLogOnAdd(userPaymentLog);

                return await this.storageBroker.InsertUserPaymentLogAsync(userPaymentLog);
            }
            catch (NullUserPaymentLogException nullUserPaymentLogException)
            {
                var userPaymentLogValidationException =
                    new UserPaymentLogValidationException(nullUserPaymentLogException);

                this.loggingBroker.LogError(userPaymentLogValidationException);

                throw userPaymentLogValidationException;
            }
            catch (InvalidUserPaymentLogException invalidUserPaymentLogException)
            {
                var userPaymentLogValidationException =
                    new UserPaymentLogValidationException(invalidUserPaymentLogException);

                this.loggingBroker.LogError(userPaymentLogValidationException);

                throw userPaymentLogValidationException;
            }
            catch (SqlException sqlException)
            {
                var failedUserPaymentLogStorageException =
                    new FailedUserPaymentLogStorageException(sqlException);

                var userPaymentLogDependencyException =
                    new UserPaymentLogDependencyException(failedUserPaymentLogStorageException);

                this.loggingBroker.LogCritical(userPaymentLogDependencyException);

                throw userPaymentLogDependencyException;
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsUserPaymentLogException =
                    new AlreadyExistsUserPaymentLogException(duplicateKeyException);

                var userPaymentLogDependencyValidationException =
                    new UserPaymentLogDependencyValidationException(alreadyExistsUserPaymentLogException);

                this.loggingBroker.LogError(userPaymentLogDependencyValidationException);

                throw userPaymentLogDependencyValidationException;
            }
            catch (Exception exception)
            {
                var failedUserPaymentLogServiceException =
                    new FailedUserPaymentLogServiceException(exception);

                var userPaymentLogServiceException =
                    new UserPaymentLogServiceException(failedUserPaymentLogServiceException);

                this.loggingBroker.LogError(userPaymentLogServiceException);

                throw userPaymentLogServiceException;
            }
        }
    }
}
