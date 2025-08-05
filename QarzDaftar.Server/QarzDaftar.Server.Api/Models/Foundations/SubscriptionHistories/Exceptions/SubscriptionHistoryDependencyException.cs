using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions
{
    public class SubscriptionHistoryDependencyException : Xeption
    {
        public SubscriptionHistoryDependencyException(Exception innerException)
            : base(message: " Subscription history dependency error occurred, contact support.",
                  innerException)
        { }
    }
}
