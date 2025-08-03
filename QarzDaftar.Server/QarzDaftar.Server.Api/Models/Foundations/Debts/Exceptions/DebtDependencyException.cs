using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions
{
    public class DebtDependencyException : Xeption
    {
        public DebtDependencyException(Exception innerException)
            : base(message: "Debt dependency error occurred, contact support.",
                  innerException)
        { }
    }
}
