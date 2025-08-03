using Microsoft.Data.SqlClient;
using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.Debts
{
    public partial class DebtService : IDebtService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public DebtService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<Debt> AddDebtAsync(Debt debt) =>
        TryCatch(async () =>
        {
            ValidateDebtOnAdd(debt);

            return await this.storageBroker.InsertDebtAsync(debt);
        });

        public IQueryable<Debt> RetrieveAllDebts()
        {
            try
            {
                return this.storageBroker.SelectAllDebts();
            }
            catch (SqlException sqlException)
            {
                var failedDebtStorageException =
                    new FailedDebtStorageException(sqlException);

                var debtDependencyException =
                    new DebtDependencyException(failedDebtStorageException);

                this.loggingBroker.LogCritical(debtDependencyException);

                throw debtDependencyException;
            }
        }
    }
}
