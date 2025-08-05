using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions;

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

        public ValueTask<SubscriptionHistory> AddSubscriptionHistoryAsync(
            SubscriptionHistory subscriptionHistory) => TryCatch(async () =>
        {
            ValidateSubscriptionHistoryOnAdd(subscriptionHistory);

            return await this.storageBroker
                .InsertSubscriptionHistoryAsync(subscriptionHistory);
        });

        public IQueryable<SubscriptionHistory> RetrieveAllSubscriptionHistories() =>
            TryCatch(() => this.storageBroker.SelectAllSubscriptionHistories());

        public async ValueTask<SubscriptionHistory> RetrieveSubscriptionHistoryByIdAsync(
            Guid subscriptionHistoryId)
        {
            try
            {
                ValidateSubscriptionHistoryId(subscriptionHistoryId);

                SubscriptionHistory maybeSubscriptionHistory =
                    await this.storageBroker.SelectSubscriptionHistoryByIdAsync(subscriptionHistoryId);

                ValidateStorageSubscriptionHistory(maybeSubscriptionHistory, subscriptionHistoryId);

                return maybeSubscriptionHistory;
            }
            catch (InvalidSubscriptionHistoryException invalidSubscriptionHistoryException)
            {
                var subscriptionHistoryValidationException =
                    new SubscriptionHistoryValidationException(invalidSubscriptionHistoryException);

                this.loggingBroker.LogError(subscriptionHistoryValidationException);

                throw subscriptionHistoryValidationException;
            }
            catch (NotFoundSubscriptionHistoryException notFoundSubscriptionHistoryException)
            {
                var subscriptionHistoryValidationException =
                    new SubscriptionHistoryValidationException(notFoundSubscriptionHistoryException);

                this.loggingBroker.LogError(subscriptionHistoryValidationException);

                throw subscriptionHistoryValidationException;
            }
        }
    }
}
