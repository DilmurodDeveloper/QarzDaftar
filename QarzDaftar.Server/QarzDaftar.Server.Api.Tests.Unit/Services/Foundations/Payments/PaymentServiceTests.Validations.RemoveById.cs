using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Payments
{
    public partial class PaymentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given 
            Guid invalidPaymentId = Guid.Empty;
            var invalidPaymentException = new InvalidPaymentException();

            invalidPaymentException.AddData(
                key: nameof(Payment.Id),
                values: "Id is required");

            var expectedPaymentValidationException =
                new PaymentValidationException(invalidPaymentException);

            // when
            ValueTask<Payment> removePaymentById =
                this.paymentService.RemovePaymentByIdAsync(invalidPaymentId);

            PaymentValidationException actualPaymentValidationException =
                await Assert.ThrowsAsync<PaymentValidationException>(
                    removePaymentById.AsTask);
            // then
            actualPaymentValidationException.Should()
                .BeEquivalentTo(expectedPaymentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.storageBrokerMock.Verify(broker =>
            broker.DeletePaymentAsync(It.IsAny<Payment>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemovePaymentByIdIsNotFoundAndLogItAsync()
        {
            // given
            Guid inputPaymentId = Guid.NewGuid();
            Payment noPayment = null;
            var notFoundPaymentException = new NotFoundPaymentException(inputPaymentId);

            var expectedPaymentValidationException =
                new PaymentValidationException(notFoundPaymentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noPayment);

            // when
            ValueTask<Payment> removePaymentById =
                this.paymentService.RemovePaymentByIdAsync(inputPaymentId);

            var actualPaymentValidationException =
                await Assert.ThrowsAsync<PaymentValidationException>(
                    removePaymentById.AsTask);

            // then
            actualPaymentValidationException.Should()
                .BeEquivalentTo(expectedPaymentValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeletePaymentAsync(It.IsAny<Payment>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
