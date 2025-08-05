using Microsoft.EntityFrameworkCore;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;

namespace QarzDaftar.Server.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<SubscriptionHistory> SubscriptionHistories { get; set; }

        public async ValueTask<SubscriptionHistory> InsertSubscriptionHistoryAsync(
            SubscriptionHistory subscriptionHistory) => await InsertAsync(subscriptionHistory);
    }
}
