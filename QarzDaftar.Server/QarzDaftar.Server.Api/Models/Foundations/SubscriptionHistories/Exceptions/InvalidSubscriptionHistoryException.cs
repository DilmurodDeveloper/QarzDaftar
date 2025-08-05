using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions
{
    public class InvalidSubscriptionHistoryException : Xeption
    {
        public InvalidSubscriptionHistoryException()
            : base(message: "Subscription History is invalid.")
        { }
    }
}
