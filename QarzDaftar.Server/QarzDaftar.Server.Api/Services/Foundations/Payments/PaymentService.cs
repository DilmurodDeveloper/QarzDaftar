using Microsoft.Data.SqlClient;
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

        public IQueryable<Payment> RetrieveAllPayments()
        {
            try
            {
                return this.storageBroker.SelectAllPayments();
            }
            catch (SqlException sqlException)
            {
                var failedPaymentStorageException =
                    new FailedPaymentStorageException(sqlException);

                var paymentDependencyException =
                    new PaymentDependencyException(failedPaymentStorageException);

                this.loggingBroker.LogCritical(paymentDependencyException);
                
                throw paymentDependencyException;
            }
        }
    }
}
