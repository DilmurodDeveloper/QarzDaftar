using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions
{
    public class SubscriptionHistoryValidationException : Xeption
    {
        public SubscriptionHistoryValidationException(Xeption innerException)
            : base(message: "Subscription History validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
