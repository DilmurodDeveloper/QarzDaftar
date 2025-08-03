using QarzDaftar.Server.Api.Models.Enums;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.Debts
{
    public partial class DebtService
    {
        private void ValidateDebtOnAdd(Debt debt)
        {
            ValidateDebtNotNull(debt);

            Validate(
                (Rule: IsInvalid(debt.Id), Parameter: nameof(Debt.Id)),
                (Rule: IsInvalid(debt.Amount), Parameter: nameof(Debt.Amount)),
                (Rule: IsInvalid(debt.Description), Parameter: nameof(Debt.Description)),
                (Rule: IsInvalid(debt.Status), Parameter: nameof(Debt.Status)),
                (Rule: IsInvalid(debt.DueDate), Parameter: nameof(Debt.DueDate)),
                (Rule: IsInvalid(debt.CreatedDate), Parameter: nameof(Debt.CreatedDate)),
                (Rule: IsInvalid(debt.UpdatedDate), Parameter: nameof(Debt.UpdatedDate)),
                (Rule: IsInvalid(debt.CustomerId), Parameter: nameof(Debt.CustomerId)),
                (Rule: IsInvalid(debt.UserId), Parameter: nameof(Debt.UserId)));
        }

        private static void ValidateDebtNotNull(Debt debt)
        {
            if (debt is null)
            {
                throw new NullDebtException();
            }
        }

        private static dynamic IsInvalid(Guid Id) => new
        {
            Condition = Id == default,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(decimal amount) => new
        {
            Condition = amount <= 0,
            Message = "Amount is required"
        };

        private static dynamic IsInvalid(DebtStatus status) => new
        {
            Condition = Enum.IsDefined(typeof(DebtStatus), status) is false,
            Message = "Value is invalid"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private void ValidateDebtId(Guid debtId) =>
            Validate((Rule: IsInvalid(debtId), Parameter: nameof(Debt.Id)));

        private static void ValidateStorageDebt(Debt maybeDebt, Guid debtId)
        {
            if (maybeDebt is null)
            {
                throw new NotFoundDebtException(debtId);
            }
        }

        private static void ValidateDebtOnModify(Debt debt)
        {
            ValidateDebtNotNull(debt);

            Validate(
                (Rule: IsInvalid(debt.Id), Parameter: nameof(Debt.Id)),
                (Rule: IsInvalid(debt.Amount), Parameter: nameof(Debt.Amount)),
                (Rule: IsInvalid(debt.Description), Parameter: nameof(Debt.Description)),
                (Rule: IsInvalid(debt.Status), Parameter: nameof(Debt.Status)),
                (Rule: IsInvalid(debt.DueDate), Parameter: nameof(Debt.DueDate)),
                (Rule: IsInvalid(debt.CreatedDate), Parameter: nameof(Debt.CreatedDate)),
                (Rule: IsInvalid(debt.UpdatedDate), Parameter: nameof(Debt.UpdatedDate)),
                (Rule: IsInvalid(debt.CustomerId), Parameter: nameof(Debt.CustomerId)),
                (Rule: IsInvalid(debt.UserId), Parameter: nameof(Debt.UserId)));
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidDebtException = new InvalidDebtException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidDebtException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidDebtException.ThrowIfContainsErrors();
        }
    }
}
