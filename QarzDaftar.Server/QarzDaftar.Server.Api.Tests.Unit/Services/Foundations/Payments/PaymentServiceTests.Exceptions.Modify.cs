using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Payments
{
    public partial class PaymentServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Payment randomPayment = CreateRandomPayment();
            Payment somePayment = randomPayment;
            Guid PaymentId = somePayment.Id;
            SqlException sqlException = GetSqlError();

            var failedPaymentStorageException =
                new FailedPaymentStorageException(sqlException);

            var expectedPaymentDependencyException =
                new PaymentDependencyException(failedPaymentStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(PaymentId)).Throws(sqlException);

            // when
            ValueTask<Payment> modifyPaymentTask =
                this.paymentService.ModifyPaymentAsync(somePayment);

            PaymentDependencyException actualPaymentDependencyException =
                 await Assert.ThrowsAsync<PaymentDependencyException>(
                    modifyPaymentTask.AsTask);

            // then
            actualPaymentDependencyException.Should()
                .BeEquivalentTo(expectedPaymentDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPaymentDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(PaymentId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePaymentAsync(somePayment), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            Payment randomPayment = CreateRandomPayment();
            Payment somePayment = randomPayment;
            Guid PaymentId = somePayment.Id;
            var databaseUpdateException = new DbUpdateException();

            var failedPaymentStorageException =
                new FailedPaymentStorageException(databaseUpdateException);

            var expectedPaymentDependencyException =
                new PaymentDependencyException(failedPaymentStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(PaymentId))
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Payment> modifyPaymentTask =
                this.paymentService.ModifyPaymentAsync(somePayment);

            PaymentDependencyException actualPaymentDependencyException =
                 await Assert.ThrowsAsync<PaymentDependencyException>(
                    modifyPaymentTask.AsTask);

            // then
            actualPaymentDependencyException.Should()
                .BeEquivalentTo(expectedPaymentDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(PaymentId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePaymentAsync(somePayment), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Payment randomPayment = CreateRandomPayment();
            Payment somePayment = randomPayment;
            Guid PaymentId = somePayment.Id;
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedPaymentException =
                new LockedPaymentException(dbUpdateConcurrencyException);

            var expectedPaymentDependencyValidationException =
                new PaymentDependencyValidationException(lockedPaymentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(PaymentId))
                    .Throws(dbUpdateConcurrencyException);

            // when
            ValueTask<Payment> modifyPaymentTask =
                this.paymentService.ModifyPaymentAsync(somePayment);

            PaymentDependencyValidationException actualPaymentDependencyValidationException =
                 await Assert.ThrowsAsync<PaymentDependencyValidationException>(
                    modifyPaymentTask.AsTask);

            // then
            actualPaymentDependencyValidationException.Should()
                .BeEquivalentTo(expectedPaymentDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(PaymentId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePaymentAsync(somePayment), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Payment randomPayment = CreateRandomPayment();
            Payment somePayment = randomPayment;
            Guid PaymentId = somePayment.Id;
            var serviceException = new Exception();

            var failedPaymentServiceException =
                new FailedPaymentServiceException(serviceException);

            var expectedPaymentServiceException =
                new PaymentServiceException(failedPaymentServiceException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectPaymentByIdAsync(PaymentId))
                .Throws(serviceException);

            // when
            ValueTask<Payment> modifyPaymentTask =
                this.paymentService.ModifyPaymentAsync(somePayment);

            PaymentServiceException actualPaymentServiceException =
                await Assert.ThrowsAsync<PaymentServiceException>(
                    modifyPaymentTask.AsTask);

            // then
            actualPaymentServiceException.Should()
                .BeEquivalentTo(expectedPaymentServiceException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentServiceException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(PaymentId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePaymentAsync(somePayment), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
