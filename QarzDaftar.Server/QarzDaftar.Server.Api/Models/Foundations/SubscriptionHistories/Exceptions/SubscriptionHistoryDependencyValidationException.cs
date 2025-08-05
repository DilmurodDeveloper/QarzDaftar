using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions
{
    public class SubscriptionHistoryDependencyValidationException : Xeption
    {
        public SubscriptionHistoryDependencyValidationException(Xeption innerException)
            : base(message: "Subscription History dependency validation error occurred, fix the errors and try again.",
                 innerException)
        { }
    }
}
