using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions
{
    public class NullSubscriptionHistoryException : Xeption
    {
        public NullSubscriptionHistoryException()
            : base(message: "Subscription History is null.")
        { }
    }
}
