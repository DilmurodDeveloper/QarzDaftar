using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;

namespace QarzDaftar.Server.Api.Services.Foundations.SubscriptionHistories
{
    public interface ISubscriptionHistoryService
    {
        ValueTask<SubscriptionHistory> AddSubscriptionHistoryAsync(SubscriptionHistory subscriptionHistory);
        IQueryable<SubscriptionHistory> RetrieveAllSubscriptionHistories();
        ValueTask<SubscriptionHistory> RetrieveSubscriptionHistoryByIdAsync(Guid subscriptionHistoryId);
    }
}
