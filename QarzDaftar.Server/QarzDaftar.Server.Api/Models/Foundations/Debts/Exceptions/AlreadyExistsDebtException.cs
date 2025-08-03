using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions
{
    public class AlreadyExistsDebtException : Xeption
    {
        public AlreadyExistsDebtException(Exception innerException)
            : base(message: "Debt already exists.", innerException)
        { }
    }
}
