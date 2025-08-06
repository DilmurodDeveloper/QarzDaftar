using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions
{
    public class FailedUserPaymentLogStorageException : Xeption
    {
        public FailedUserPaymentLogStorageException(Exception innerException)
            : base(message: "Failed user payment log storage error occurred, contact support.",
                  innerException)
        { }
    }
}
