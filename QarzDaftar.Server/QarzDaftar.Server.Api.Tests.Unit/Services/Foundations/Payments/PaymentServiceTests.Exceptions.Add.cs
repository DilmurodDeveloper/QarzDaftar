using Microsoft.Data.SqlClient;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Payments
{
    public partial class PaymentServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursLogItAsync()
        {
            // given
            Payment somePayment = CreateRandomPayment();
            SqlException sqlException = GetSqlError();

            var failedPaymentStorageException =
                new FailedPaymentStorageException(sqlException);

            var expectedPaymentDependencyException =
                new PaymentDependencyException(failedPaymentStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertPaymentAsync(somePayment))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Payment> addPaymentTask =
                this.paymentService.AddPaymentAsync(somePayment);

            // then
            await Assert.ThrowsAsync<PaymentDependencyException>(() =>
                addPaymentTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPaymentAsync(somePayment), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPaymentDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
