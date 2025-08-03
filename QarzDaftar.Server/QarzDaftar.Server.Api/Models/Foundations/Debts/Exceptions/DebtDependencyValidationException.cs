using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions
{
    public class DebtDependencyValidationException : Xeption
    {
        public DebtDependencyValidationException(Xeption innerException)
            : base(message: "Debt dependency validation error occurred, fix the errors and try again.",
                 innerException)
        { }
    }
}
