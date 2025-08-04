using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Payments;
using QarzDaftar.Server.Api.Models.Foundations.Payments.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Payments
{
    public partial class PaymentServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            Payment nullPayment = null;
            var nullPaymentException = new NullPaymentException();

            var expectedPaymentValidationException =
                new PaymentValidationException(nullPaymentException);

            // when
            ValueTask<Payment> addPaymentTask =
                this.paymentService.AddPaymentAsync(nullPayment);

            PaymentValidationException actualPaymentException =
                await Assert.ThrowsAsync<PaymentValidationException>(
                    addPaymentTask.AsTask);

            // then
            actualPaymentException.Should()
                .BeEquivalentTo(expectedPaymentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPaymentAsync(It.IsAny<Payment>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfPaymentIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidPayment = new Payment
            {
                Description = invalidText,
            };

            var invalidPaymentException = new InvalidPaymentException();

            invalidPaymentException.AddData(
                key: nameof(Payment.Id),
                values: "Id is required");

            invalidPaymentException.AddData(
                key: nameof(Payment.Amount),
                values: "Amount is required");

            invalidPaymentException.AddData(
                key: nameof(Payment.Description),
                values: "Text is required");

            invalidPaymentException.AddData(
                key: nameof(Payment.PaymentDate),
                values: "Date is required");

            invalidPaymentException.AddData(
                key: nameof(Payment.CreatedDate),
                values: "Date is required");

            invalidPaymentException.AddData(
                key: nameof(Payment.UpdatedDate),
                values: "Date is required");

            invalidPaymentException.AddData(
                key: nameof(Payment.CustomerId),
                values: "Id is required");

            var expectedPaymentValidationException =
                new PaymentValidationException(invalidPaymentException);

            // when
            ValueTask<Payment> addPaymentTask =
                this.paymentService.AddPaymentAsync(invalidPayment);

            PaymentValidationException actualPaymentValidationException =
                await Assert.ThrowsAsync<PaymentValidationException>(
                    addPaymentTask.AsTask);

            // then
            actualPaymentValidationException.Should()
                .BeEquivalentTo(expectedPaymentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPaymentAsync(It.IsAny<Payment>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
