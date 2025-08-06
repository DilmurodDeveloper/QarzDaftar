using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions
{
    public class NotFoundUserPaymentLogException : Xeption
    {
        public NotFoundUserPaymentLogException(Guid userPaymentLogId)
            : base(message: $"User payment log is not found with id: {userPaymentLogId}")
        { }
    }
}
