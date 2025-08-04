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
        public async Task ShouldThrowSqlExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedPaymentStorageException =
                new FailedPaymentStorageException(sqlException);

            var expectedPaymentDependencyException =
                new PaymentDependencyException(failedPaymentStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Payment> retrievePaymentByIdTask =
                this.paymentService.RetrievePaymentByIdAsync(someId);

            PaymentDependencyException actualPaymentDependencyException =
                await Assert.ThrowsAsync<PaymentDependencyException>(
                    retrievePaymentByIdTask.AsTask);

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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedPaymentServiceException =
                new FailedPaymentServiceException(serviceException);

            var expectedPaymentServiceException =
                new PaymentServiceException(failedPaymentServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Payment> retrievePaymentByIdTask =
                this.paymentService.RetrievePaymentByIdAsync(someId);

            PaymentServiceException actualPaymentServiceException =
                await Assert.ThrowsAsync<PaymentServiceException>(
                    retrievePaymentByIdTask.AsTask);

            // then
            actualPaymentServiceException.Should()
                .BeEquivalentTo(expectedPaymentServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

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
