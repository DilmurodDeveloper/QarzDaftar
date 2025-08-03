using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions
{
    public class DebtValidationException : Xeption
    {
        public DebtValidationException(Xeption innerException)
            : base(message: "Debt validation error occurred, fix the errors and try again.",
                  innerException)
        { }
    }
}
