using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<SubscriptionHistory> InsertSubscriptionHistoryAsync(SubscriptionHistory subscriptionHistory);
        IQueryable<SubscriptionHistory> SelectAllSubscriptionHistories();
        ValueTask<SubscriptionHistory> SelectSubscriptionHistoryByIdAsync(Guid subscriptionHistoryId);
        ValueTask<SubscriptionHistory> UpdateSubscriptionHistoryAsync(SubscriptionHistory subscriptionHistory);
        ValueTask<SubscriptionHistory> DeleteSubscriptionHistoryAsync(SubscriptionHistory subscriptionHistory);
    }
}
