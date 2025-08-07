using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Services.Foundations.Customers;
using QarzDaftar.Server.Api.Services.Foundations.Payments;
using QarzDaftar.Server.Api.Services.Processings.Debts;

namespace QarzDaftar.Server.Api.Services.Processings.Payments
{
    public partial class PaymentProcessingService : IPaymentProcessingService
    {
        private readonly IPaymentService paymentService;
        private readonly ICustomerService customerService;
        private readonly IDebtProcessingService debtProcessingService;

        public PaymentProcessingService(
            IPaymentService paymentService,
            ICustomerService customerService,
            IDebtProcessingService debtProcessingService)
        {
            this.paymentService = paymentService;
            this.customerService = customerService;
            this.debtProcessingService = debtProcessingService;
        }

        public async ValueTask<Payment> AddPaymentAsync(Payment payment, Guid userId)
        {
            Customer customer =
                await customerService.RetrieveCustomerByIdAsync(payment.CustomerId);

            if (customer.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to add payment to this customer.");

            payment.Id = Guid.NewGuid();
            payment.CreatedDate = DateTimeOffset.UtcNow;
            payment.UpdatedDate = DateTimeOffset.UtcNow;

            Payment addedPayment = await paymentService.AddPaymentAsync(payment);

            return addedPayment;
        }

        public IQueryable<Payment> RetrieveAllPaymentsByUserId(Guid userId) =>
            paymentService.RetrieveAllPayments()
                .Where(payment => payment.Customer!.UserId == userId);

        public async ValueTask<Payment> RetrievePaymentByIdAsync(Guid paymentId) =>
            await paymentService.RetrievePaymentByIdAsync(paymentId);

        public async ValueTask<Payment> ModifyPaymentAsync(Payment payment, Guid userId)
        {
            Customer customer =
                await customerService.RetrieveCustomerByIdAsync(payment.CustomerId);

            if (customer.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to modify this payment.");

            payment.UpdatedDate = DateTimeOffset.UtcNow;

            return await paymentService.ModifyPaymentAsync(payment);
        }

        public async ValueTask<Payment> RemovePaymentByIdAsync(Guid paymentId, Guid userId)
        {
            Payment payment = await paymentService.RetrievePaymentByIdAsync(paymentId);

            Customer customer = await customerService.RetrieveCustomerByIdAsync(payment.CustomerId);

            if (customer.UserId != userId)
                throw new UnauthorizedAccessException("You are not authorized to delete this payment.");

            return await paymentService.RemovePaymentByIdAsync(paymentId);
        }

        public decimal CalculateTotalPaidAmountForCustomer(Guid customerId)
        {
            var payments = paymentService.RetrieveAllPaymentsByCustomerId(customerId);
            return payments.Sum(p => p.Amount);
        }

        public decimal CalculateRemainingDebtForCustomer(Guid customerId)
        {
            decimal totalDebt = debtProcessingService.CalculateTotalDebtForCustomer(customerId);
            decimal totalPaid = CalculateTotalPaidAmountForCustomer(customerId);
            return totalDebt - totalPaid;
        }

        public decimal CalculateTotalPaidByUserId(Guid userId)
        {
            var payments = paymentService.RetrieveAllPayments()
                .Where(payment => payment.Customer.UserId == userId);

            return payments.Sum(payment => payment.Amount);
        }

        public decimal CalculateRemainingDebtByUserId(Guid userId)
        {
            decimal totalDebt = debtProcessingService.CalculateTotalDebtByUserId(userId);
            decimal totalPaid = CalculateTotalPaidByUserId(userId);
            return totalDebt - totalPaid;
        }
    }
}
