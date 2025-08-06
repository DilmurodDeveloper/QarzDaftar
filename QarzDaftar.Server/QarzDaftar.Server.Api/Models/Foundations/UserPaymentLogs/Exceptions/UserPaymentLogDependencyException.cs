using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions
{
    public class UserPaymentLogDependencyException : Xeption
    {
        public UserPaymentLogDependencyException(Exception innerException)
            : base(message: "User payment log dependency error occurred, contact support.",
                  innerException)
        { }
    }
}
