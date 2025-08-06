using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions
{
    public class UserPaymentLogValidationException : Xeption
    {
        public UserPaymentLogValidationException(Xeption innerException)
            : base(message: "User payment log validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
