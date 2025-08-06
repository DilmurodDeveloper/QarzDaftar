using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions
{
    public class UserPaymentLogDependencyValidationException : Xeption
    {
        public UserPaymentLogDependencyValidationException(Xeption innerException)
            : base(message: "User payment log dependency validation error occurred, fix the errors and try again.",
                 innerException)
        { }
    }
}
