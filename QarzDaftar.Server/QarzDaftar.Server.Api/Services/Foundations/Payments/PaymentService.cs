using QarzDaftar.Server.Api.Brokers.DateTimes;
using QarzDaftar.Server.Api.Brokers.Loggings;
using QarzDaftar.Server.Api.Brokers.Storages;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.Payments
{
    public partial class PaymentService : IPaymentService
    {
        private readonly IStorageBroker storageBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IDateTimeBroker dateTimeBroker;

        public PaymentService(
            IStorageBroker storageBroker,
            ILoggingBroker loggingBroker,
            IDateTimeBroker dateTimeBroker)
        {
            this.storageBroker = storageBroker;
            this.loggingBroker = loggingBroker;
            this.dateTimeBroker = dateTimeBroker;
        }

        public ValueTask<Payment> AddPaymentAsync(Payment payment) =>
        TryCatch(async () =>
        {
            ValidatePaymentOnAdd(payment);

            return await this.storageBroker.InsertPaymentAsync(payment);
        });

        public IQueryable<Payment> RetrieveAllPayments() =>
            TryCatch(() => this.storageBroker.SelectAllPayments());

        public async ValueTask<Payment> RetrievePaymentByIdAsync(Guid paymentId)
        {
            try
            {
                ValidatePaymentId(paymentId);

                Payment maybePayment =
                    await this.storageBroker.SelectPaymentByIdAsync(paymentId);

                ValidateStoragePayment(maybePayment, paymentId);

                return maybePayment;
            }
            catch (InvalidPaymentException invalidPaymentException)
            {
                var paymentValidationException =
                    new PaymentValidationException(invalidPaymentException);

                this.loggingBroker.LogError(paymentValidationException);

                throw paymentValidationException;
            }
            catch (NotFoundPaymentException notFoundPaymentException)
            {
                var paymentValidationException =
                    new PaymentValidationException(notFoundPaymentException);

                this.loggingBroker.LogError(paymentValidationException);

                throw paymentValidationException;
            }
        }
    }
}
