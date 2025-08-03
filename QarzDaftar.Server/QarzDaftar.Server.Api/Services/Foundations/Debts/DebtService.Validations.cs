using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.Debts
{
    public partial class DebtService
    {
        private static void ValidateDebtNotNull(Debt debt)
        {
            if (debt is null)
            {
                throw new NullDebtException();
            }
        }
    }
}
