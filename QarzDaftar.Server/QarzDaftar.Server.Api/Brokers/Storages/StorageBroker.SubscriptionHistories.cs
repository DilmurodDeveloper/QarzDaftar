using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<SubscriptionHistory> SubscriptionHistories { get; set; }

        public async ValueTask<SubscriptionHistory> InsertSubscriptionHistoryAsync(
            SubscriptionHistory subscriptionHistory) => await InsertAsync(subscriptionHistory);

        public IQueryable<SubscriptionHistory> SelectAllSubscriptionHistories()
        {
            var subscriptionHistorys = SelectAll<SubscriptionHistory>()
                .Include(c => c.User);

            return subscriptionHistorys;
        }

        public async ValueTask<SubscriptionHistory> SelectSubscriptionHistoryByIdAsync(Guid subscriptionHistoryId)
        {
            var subscriptionHistoryWithDetails = SubscriptionHistories
                .Include(c => c.User)
                .FirstOrDefault(c => c.Id == subscriptionHistoryId);

            return await ValueTask.FromResult(subscriptionHistoryWithDetails);
        }

        public async ValueTask<SubscriptionHistory> UpdateSubscriptionHistoryAsync(
            SubscriptionHistory subscriptionHistory) => await UpdateAsync(subscriptionHistory);
    }
}
