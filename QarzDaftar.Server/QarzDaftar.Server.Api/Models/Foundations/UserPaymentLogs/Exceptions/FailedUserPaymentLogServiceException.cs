using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions
{
    public class FailedUserPaymentLogServiceException : Xeption
    {
        public FailedUserPaymentLogServiceException(Exception innerException)
            : base(message: "Failed user payment log service error occurred, please contact support.",
                  innerException)
        { }
    }
}
