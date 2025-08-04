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
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid somePaymentId = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedPaymentException =
                new LockedPaymentException(dbUpdateConcurrencyException);

            var expectedPaymentDependencyValidationException =
                new PaymentDependencyValidationException(lockedPaymentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Payment> removePaymentById =
                this.paymentService.RemovePaymentByIdAsync(somePaymentId);

            var actualPaymentDependencyValidationException =
                await Assert.ThrowsAsync<PaymentDependencyValidationException>(
                    removePaymentById.AsTask);

            // then
            actualPaymentDependencyValidationException.Should()
                .BeEquivalentTo(expectedPaymentDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeletePaymentAsync(It.IsAny<Payment>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid somePaymentId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedPaymentStorageException =
                new FailedPaymentStorageException(sqlException);

            var expectedPaymentDependencyException =
                new PaymentDependencyException(failedPaymentStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);
            // when
            ValueTask<Payment> deletePaymentTask =
                this.paymentService.RemovePaymentByIdAsync(somePaymentId);

            PaymentDependencyException actualPaymentDependencyException =
                await Assert.ThrowsAsync<PaymentDependencyException>(
                    deletePaymentTask.AsTask);

            // then
            actualPaymentDependencyException.Should()
                .BeEquivalentTo(expectedPaymentDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPaymentDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid somePaymentId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedPaymentServiceException =
                new FailedPaymentServiceException(serviceException);

            var expectedPaymentServiceException =
                new PaymentServiceException(failedPaymentServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Payment> removePaymentByIdTask =
                this.paymentService.RemovePaymentByIdAsync(somePaymentId);

            PaymentServiceException actualPaymentServiceException =
                await Assert.ThrowsAsync<PaymentServiceException>(
                    removePaymentByIdTask.AsTask);

            // then
            actualPaymentServiceException.Should()
                .BeEquivalentTo(expectedPaymentServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
