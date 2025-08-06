using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions
{
    public class LockedUserPaymentLogException : Xeption
    {
        public LockedUserPaymentLogException(Exception innerException)
            : base(message: "User payment log is locked, try again later.",
                  innerException)
        { }
    }
}
