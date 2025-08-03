using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions
{
    public class NotFoundDebtException : Xeption
    {
        public NotFoundDebtException(Guid debtId)
            : base(message: $"Debt is not found with id: {debtId}")
        { }
    }
}
