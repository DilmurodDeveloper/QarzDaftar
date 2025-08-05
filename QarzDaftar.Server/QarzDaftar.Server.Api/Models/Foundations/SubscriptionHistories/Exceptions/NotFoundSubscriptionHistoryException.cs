using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions
{
    public class NotFoundSubscriptionHistoryException : Xeption
    {
        public NotFoundSubscriptionHistoryException(Guid subscriptionHistoryId)
            : base(message: $"Subscription History is not found with id: {subscriptionHistoryId}")
        { }
    }
}
