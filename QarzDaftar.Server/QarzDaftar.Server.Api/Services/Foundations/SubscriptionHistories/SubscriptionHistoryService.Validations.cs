using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryService
    {
        private static void ValidateSubscriptionHistoryNotNull(
            SubscriptionHistory subscriptionHistory)
        {
            if (subscriptionHistory is null)
            {
                throw new NullSubscriptionHistoryException();
            }
        }
    }
}
