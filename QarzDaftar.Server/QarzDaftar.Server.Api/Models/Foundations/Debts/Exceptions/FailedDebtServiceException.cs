using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions
{
    public class FailedDebtServiceException : Xeption
    {
        public FailedDebtServiceException(Exception innerException)
            : base(message: "Failed debt service error occurred, please contact support.",
                  innerException)
        { }
    }
}
