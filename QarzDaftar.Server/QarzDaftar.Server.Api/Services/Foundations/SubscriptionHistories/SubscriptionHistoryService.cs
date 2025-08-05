using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;

namespace QarzDaftar.Server.Api.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryService : ISubscriptionHistoryService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public SubscriptionHistoryService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<SubscriptionHistory> AddSubscriptionHistoryAsync(SubscriptionHistory subscriptionHistory) =>
        TryCatch(async () =>
        {
            ValidateSubscriptionHistoryOnAdd(subscriptionHistory);

            return await this.storageBroker
                .InsertSubscriptionHistoryAsync(subscriptionHistory);
        });

        public IQueryable<SubscriptionHistory> RetrieveAllSubscriptionHistories() =>
            TryCatch(() => this.storageBroker.SelectAllSubscriptionHistories());

        public ValueTask<SubscriptionHistory> RetrieveSubscriptionHistoryByIdAsync(Guid subscriptionHistoryId) =>
        TryCatch(async () =>
        {
            ValidateSubscriptionHistoryId(subscriptionHistoryId);

            SubscriptionHistory maybeSubscriptionHistory =
                await this.storageBroker.SelectSubscriptionHistoryByIdAsync(subscriptionHistoryId);

            ValidateStorageSubscriptionHistory(maybeSubscriptionHistory, subscriptionHistoryId);

            return maybeSubscriptionHistory;
        });

        public ValueTask<SubscriptionHistory> ModifySubscriptionHistoryAsync(SubscriptionHistory subscriptionHistory) =>
        TryCatch(async () =>
        {
            ValidateSubscriptionHistoryOnModify(subscriptionHistory);

            SubscriptionHistory maybeSubscriptionHistory =
                await this.storageBroker.SelectSubscriptionHistoryByIdAsync(subscriptionHistory.Id);

            ValidateAgainstStorageSubscriptionHistoryOnModify(subscriptionHistory, maybeSubscriptionHistory);

            return await this.storageBroker.UpdateSubscriptionHistoryAsync(subscriptionHistory);
        });

        public ValueTask<SubscriptionHistory> RemoveSubscriptionHistoryByIdAsync(Guid subscriptionHistoryId) =>
        TryCatch(async () =>
        {
            ValidateSubscriptionHistoryId(subscriptionHistoryId);

            SubscriptionHistory maybeSubscriptionHistory =
                await this.storageBroker.SelectSubscriptionHistoryByIdAsync(subscriptionHistoryId);

            ValidateStorageSubscriptionHistory(maybeSubscriptionHistory, subscriptionHistoryId);

            return await this.storageBroker.DeleteSubscriptionHistoryAsync(maybeSubscriptionHistory);
        });
    }
}
