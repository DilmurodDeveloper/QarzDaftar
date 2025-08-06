using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions
{
    public class UserPaymentLogServiceException : Xeption
    {
        public UserPaymentLogServiceException(Exception innerException)
            : base(message: "User payment log service error occurred, contact support.",
                  innerException)
        { }
    }
}
