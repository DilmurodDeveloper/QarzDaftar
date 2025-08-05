using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<SubscriptionHistory> InsertSubscriptionHistoryAsync(SubscriptionHistory subscriptionHistory);
    }
}
