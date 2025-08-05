using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions
{
    public class SubscriptionHistoryServiceException : Xeption
    {
        public SubscriptionHistoryServiceException(Exception innerException)
            : base(message: "Subscription history service error occurred, contact support.",
                  innerException)
        { }
    }
}
