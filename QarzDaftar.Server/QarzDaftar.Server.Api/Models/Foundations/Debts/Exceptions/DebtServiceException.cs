using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions
{
    public class DebtServiceException : Xeption
    {
        public DebtServiceException(Exception innerException)
            : base(message: "Debt service error occurred, contact support.",
                  innerException)
        { }
    }
}
