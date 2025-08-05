using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions
{
    public class FailedSubscriptionHistoryServiceException : Xeption
    {
        public FailedSubscriptionHistoryServiceException(Exception innerException)
            : base(message: "Failed subscription history service error occurred, please contact support.",
                  innerException)
        { }
    }
}
