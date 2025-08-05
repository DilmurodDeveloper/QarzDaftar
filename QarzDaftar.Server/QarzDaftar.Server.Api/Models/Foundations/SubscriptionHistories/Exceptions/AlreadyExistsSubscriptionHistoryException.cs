using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions
{
    public class AlreadyExistsSubscriptionHistoryException : Xeption
    {
        public AlreadyExistsSubscriptionHistoryException(Exception innerException)
            : base(message: "Subscription History already exists.", innerException)
        { }
    }
}
