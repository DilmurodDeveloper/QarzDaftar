using EFxceptions.Models.Exceptions;
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

        public async ValueTask<Debt> AddDebtAsync(Debt debt)
        {
            try
            {
                ValidateDebtOnAdd(debt);

                return await this.storageBroker.InsertDebtAsync(debt);
            }
            catch (NullDebtException nullDebtException)
            {
                var debtValidationException =
                    new DebtValidationException(nullDebtException);

                this.loggingBroker.LogError(debtValidationException);

                throw debtValidationException;
            }
            catch (InvalidDebtException invalidDebtException)
            {
                var debtValidationException =
                    new DebtValidationException(invalidDebtException);

                this.loggingBroker.LogError(debtValidationException);

                throw debtValidationException;
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
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsDebtException =
                    new AlreadyExistsDebtException(duplicateKeyException);

                var debtDependencyValidationException =
                    new DebtDependencyValidationException(alreadyExistsDebtException);

                this.loggingBroker.LogError(debtDependencyValidationException);

                throw debtDependencyValidationException;
            }
        }
    }
}
