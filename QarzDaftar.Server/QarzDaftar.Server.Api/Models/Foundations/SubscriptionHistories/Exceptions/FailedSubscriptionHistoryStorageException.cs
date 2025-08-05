using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions
{
    public class FailedSubscriptionHistoryStorageException : Xeption
    {
        public FailedSubscriptionHistoryStorageException(Exception innerException)
            : base(message: "Failed subscription history storage error occurred, contact support.",
                  innerException)
        { }
    }
}
