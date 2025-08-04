using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Payments
{
    public partial class PaymentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionIfPaymentIsNullAndLogItAsync()
        {
            // given
            Payment nullPayment = null;
            var nullPaymentException = new NullPaymentException();

            var expectedPaymentValidationException =
                new PaymentValidationException(nullPaymentException);

            // when
            ValueTask<Payment> modifyPaymentTask =
                this.paymentService.ModifyPaymentAsync(nullPayment);

            PaymentValidationException actualPaymentValidationException =
                await Assert.ThrowsAsync<PaymentValidationException>(
                    modifyPaymentTask.AsTask);

            // then
            actualPaymentValidationException.Should()
                .BeEquivalentTo(expectedPaymentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePaymentAsync(It.IsAny<Payment>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
