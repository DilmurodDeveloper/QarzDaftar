using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions
{
    public class LockedDebtException : Xeption
    {
        public LockedDebtException(Exception innerException)
            : base(message: "Debt is locked, try again later.",
                  innerException)
        { }
    }
}
