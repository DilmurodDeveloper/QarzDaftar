using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;

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
            UserPaymentLog maybeUserPaymentLog =
                await this.storageBroker.SelectUserPaymentLogByIdAsync(userPaymentLog.Id);

            return await this.storageBroker.UpdateUserPaymentLogAsync(userPaymentLog);
        }
    }
}
