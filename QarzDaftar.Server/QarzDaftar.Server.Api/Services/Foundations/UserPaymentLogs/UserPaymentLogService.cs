using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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

        public ValueTask<UserPaymentLog> AddUserPaymentLogAsync(UserPaymentLog userPaymentLog) =>
        TryCatch(async () =>
        {
            ValidateUserPaymentLogOnAdd(userPaymentLog);

            return await this.storageBroker.InsertUserPaymentLogAsync(userPaymentLog);
        });

        public IQueryable<UserPaymentLog> RetrieveAllUserPaymentLogs() =>
            TryCatch(() => this.storageBroker.SelectAllUserPaymentLogs());

        public ValueTask<UserPaymentLog> RetrieveUserPaymentLogByIdAsync(Guid userPaymentLogId) =>
        TryCatch(async () =>
        {
            ValidateUserPaymentLogId(userPaymentLogId);

            UserPaymentLog maybeUserPaymentLog =
                await this.storageBroker.SelectUserPaymentLogByIdAsync(userPaymentLogId);

            ValidateStorageUserPaymentLog(maybeUserPaymentLog, userPaymentLogId);

            return maybeUserPaymentLog;
        });

        public async ValueTask<UserPaymentLog> ModifyUserPaymentLogAsync(UserPaymentLog userPaymentLog)
        {
            try
            {
                ValidateUserPaymentLogOnModify(userPaymentLog);

                UserPaymentLog maybeUserPaymentLog =
                    await this.storageBroker.SelectUserPaymentLogByIdAsync(userPaymentLog.Id);

                ValidateAgainstStorageUserPaymentLogOnModify(userPaymentLog, maybeUserPaymentLog);

                return await this.storageBroker.UpdateUserPaymentLogAsync(userPaymentLog);
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
            catch (NotFoundUserPaymentLogException notFoundUserPaymentLogException)
            {
                var userPaymentLogValidationException =
                    new UserPaymentLogValidationException(notFoundUserPaymentLogException);

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
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedUserPaymentLogException =
                    new LockedUserPaymentLogException(dbUpdateConcurrencyException);

                var userPaymentLogDependencyValidationException =
                    new UserPaymentLogDependencyValidationException(lockedUserPaymentLogException);

                this.loggingBroker.LogError(userPaymentLogDependencyValidationException);

                throw userPaymentLogDependencyValidationException;
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedUserPaymentLogStorageException =
                    new FailedUserPaymentLogStorageException(dbUpdateException);

                var userPaymentLogDependencyException =
                    new UserPaymentLogDependencyException(failedUserPaymentLogStorageException);

                this.loggingBroker.LogError(userPaymentLogDependencyException);

                throw userPaymentLogDependencyException;
            }
        }
    }
}
