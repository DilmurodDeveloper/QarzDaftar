using QarzDaftar.Server.Api.Models.Enums;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.Payments
{
    public partial class PaymentService
    {
        private void ValidatePaymentOnAdd(Payment payment)
        {
            ValidatePaymentNotNull(payment);

            Validate(
                (Rule: IsInvalid(payment.Id), Parameter: nameof(Payment.Id)),
                (Rule: IsInvalid(payment.Amount), Parameter: nameof(Payment.Amount)),
                (Rule: IsInvalid(payment.Description), Parameter: nameof(Payment.Description)),
                (Rule: IsInvalid(payment.Method), Parameter: nameof(Payment.Method)),
                (Rule: IsInvalid(payment.PaymentDate), Parameter: nameof(Payment.PaymentDate)),
                (Rule: IsInvalid(payment.CreatedDate), Parameter: nameof(Payment.CreatedDate)),
                (Rule: IsInvalid(payment.UpdatedDate), Parameter: nameof(Payment.UpdatedDate)),
                (Rule: IsInvalid(payment.CustomerId), Parameter: nameof(Payment.CustomerId)));
        }

        private static void ValidatePaymentNotNull(Payment payment)
        {
            if (payment is null)
            {
                throw new NullPaymentException();
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

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(PaymentMethod method) => new
        {
            Condition = Enum.IsDefined(typeof(PaymentMethod), method) is false,
            Message = "Value is invalid"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private void ValidatePaymentId(Guid paymentId) =>
            Validate((Rule: IsInvalid(paymentId), Parameter: nameof(Payment.Id)));

        private static void ValidateStoragePayment(Payment maybePayment, Guid paymentId)
        {
            if (maybePayment is null)
            {
                throw new NotFoundPaymentException(paymentId);
            }
        }

        private void ValidatePaymentOnModify(Payment payment)
        {
            ValidatePaymentNotNull(payment);

            Validate(
                (Rule: IsInvalid(payment.Id), Parameter: nameof(Payment.Id)),
                (Rule: IsInvalid(payment.Amount), Parameter: nameof(Payment.Amount)),
                (Rule: IsInvalid(payment.Description), Parameter: nameof(Payment.Description)),
                (Rule: IsInvalid(payment.Method), Parameter: nameof(Payment.Method)),
                (Rule: IsInvalid(payment.PaymentDate), Parameter: nameof(Payment.PaymentDate)),
                (Rule: IsInvalid(payment.CreatedDate), Parameter: nameof(Payment.CreatedDate)),
                (Rule: IsInvalid(payment.UpdatedDate), Parameter: nameof(Payment.UpdatedDate)),
                (Rule: IsInvalid(payment.CustomerId), Parameter: nameof(Payment.CustomerId)));
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidPaymentException = new InvalidPaymentException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidPaymentException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidPaymentException.ThrowIfContainsErrors();
        }
    }
}
