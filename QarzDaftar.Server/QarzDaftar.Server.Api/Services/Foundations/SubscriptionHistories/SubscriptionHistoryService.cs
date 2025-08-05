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

        public async ValueTask<SubscriptionHistory> AddSubscriptionHistoryAsync(
            SubscriptionHistory subscriptionHistory)
        {
            try
            {
                ValidateSubscriptionHistoryOnAdd(subscriptionHistory);

                return await this.storageBroker
                    .InsertSubscriptionHistoryAsync(subscriptionHistory);
            }
            catch (NullSubscriptionHistoryException nullSubscriptionHistoryException)
            {
                var subscriptionHistoryValidationException =
                    new SubscriptionHistoryValidationException(nullSubscriptionHistoryException);
                
                this.loggingBroker.LogError(subscriptionHistoryValidationException);

                throw subscriptionHistoryValidationException;
            }
            catch (InvalidSubscriptionHistoryException invalidSubscriptionHistoryException)
            {
                var subscriptionHistoryValidationException =
                    new SubscriptionHistoryValidationException(invalidSubscriptionHistoryException);
                
                this.loggingBroker.LogError(subscriptionHistoryValidationException);

                throw subscriptionHistoryValidationException;
            }
        }
    }
}
