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

        public async ValueTask<Payment> AddPaymentAsync(Payment payment)
        {
            try
            {
                ValidatePaymentNotNull(payment);

                return await this.storageBroker.InsertPaymentAsync(payment);
            }
            catch (NullPaymentException nullPaymentException)
            {
                var paymentValidationException =
                    new PaymentValidationException(nullPaymentException);

                this.loggingBroker.LogError(paymentValidationException);

                throw paymentValidationException;
            }
        }
    }
}
