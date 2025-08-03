using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions
{
    public class NullDebtException : Xeption
    {
        public NullDebtException()
            : base(message: "Debt is null.")
        { }
    }
}
