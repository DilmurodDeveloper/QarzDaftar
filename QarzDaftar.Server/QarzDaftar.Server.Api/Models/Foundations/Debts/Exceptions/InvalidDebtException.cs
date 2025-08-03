using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions
{
    public class InvalidDebtException : Xeption
    {
        public InvalidDebtException()
            : base(message: "Debt is invalid.")
        { }
    }
}
