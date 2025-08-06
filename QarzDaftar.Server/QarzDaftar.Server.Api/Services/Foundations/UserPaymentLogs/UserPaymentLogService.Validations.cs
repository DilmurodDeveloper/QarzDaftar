using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogService
    {
        private void ValidateUserPaymentLogOnAdd(UserPaymentLog userPaymentLog)
        {
            ValidateUserPaymentLogNotNull(userPaymentLog);

            Validate(
                (Rule: IsInvalid(userPaymentLog.Id), Parameter: nameof(UserPaymentLog.Id)),
                (Rule: IsInvalid(userPaymentLog.Amount), Parameter: nameof(UserPaymentLog.Amount)),
                (Rule: IsInvalid(userPaymentLog.PaymentMethod), Parameter: nameof(UserPaymentLog.PaymentMethod)),
                (Rule: IsInvalid(userPaymentLog.Purpose), Parameter: nameof(UserPaymentLog.Purpose)),
                (Rule: IsInvalid(userPaymentLog.Comment), Parameter: nameof(UserPaymentLog.Comment)),
                (Rule: IsInvalid(userPaymentLog.PaidAt), Parameter: nameof(UserPaymentLog.PaidAt)),
                (Rule: IsInvalid(userPaymentLog.CreatedDate), Parameter: nameof(UserPaymentLog.CreatedDate)),
                (Rule: IsInvalid(userPaymentLog.UserId), Parameter: nameof(UserPaymentLog.UserId)));
        }

        private static void ValidateUserPaymentLogNotNull(UserPaymentLog userPaymentLog)
        {
            if (userPaymentLog is null)
            {
                throw new NullUserPaymentLogException();
            }
        }

        private static dynamic IsInvalid(Guid Id) => new
        {
            Condition = Id == default,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(decimal amount) => new
        {
            Condition = amount < 0,
            Message = "Amount must be greater than zero"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserPaymentLogException = new InvalidUserPaymentLogException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserPaymentLogException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidUserPaymentLogException.ThrowIfContainsErrors();
        }
    }
}
