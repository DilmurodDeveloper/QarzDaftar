using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.Debts;

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

        public IQueryable<Debt> RetrieveAllDebts() =>
            TryCatch(() => this.storageBroker.SelectAllDebts());

        public ValueTask<Debt> RetrieveDebtByIdAsync(Guid debtId) =>
        TryCatch(async () =>
        {
            ValidateDebtId(debtId);

            Debt maybeDebt = await this.storageBroker.SelectDebtByIdAsync(debtId);

            ValidateStorageDebt(maybeDebt, debtId);

            return maybeDebt;
        });

        public async ValueTask<Debt> ModifyDebtAsync(Debt debt)
        {
            Debt maybeDebt =
                await this.storageBroker.SelectDebtByIdAsync(debt.Id);

            return await this.storageBroker.UpdateDebtAsync(debt);
        }
    }
}
