using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions
{
    public class LockedSubscriptionHistoryException : Xeption
    {
        public LockedSubscriptionHistoryException(Exception innerException)
            : base(message: "Subscription History is locked, try again later.",
                  innerException)
        { }
    }
}
