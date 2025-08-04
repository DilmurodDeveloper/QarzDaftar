using EFxceptions.Models.Exceptions;
using FluentAssertions;
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

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccursAndLogItAsync()
        {
            // given
            string someMessage = GetRandomString();
            Payment somePayment = CreateRandomPayment();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsPaymentException =
                new AlreadyExistsPaymentException(duplicateKeyException);

            var expectedPaymentDependencyValidationException =
                new PaymentDependencyValidationException(alreadyExistsPaymentException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertPaymentAsync(somePayment)).ThrowsAsync(duplicateKeyException);
            // when
            ValueTask<Payment> addPaymentTask =
                this.paymentService.AddPaymentAsync(somePayment);

            PaymentDependencyValidationException actualPaymentDependencyValidationException =
                await Assert.ThrowsAsync<PaymentDependencyValidationException>(
                    addPaymentTask.AsTask);

            // then
            actualPaymentDependencyValidationException.Should()
                .BeEquivalentTo(expectedPaymentDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPaymentAsync(somePayment), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
