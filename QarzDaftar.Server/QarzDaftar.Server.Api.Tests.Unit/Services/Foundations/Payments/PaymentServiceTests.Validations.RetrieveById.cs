using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Payments
{
    public partial class PaymentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
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
            ValueTask<Payment> retrievePaymentById =
                this.paymentService.RetrievePaymentByIdAsync(invalidPaymentId);

            PaymentValidationException actualPaymentValidationException =
                await Assert.ThrowsAsync<PaymentValidationException>(
                    retrievePaymentById.AsTask);

            // then
            actualPaymentValidationException.Should()
                .BeEquivalentTo(expectedPaymentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
