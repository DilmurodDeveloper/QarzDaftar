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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfPaymentIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            Payment invalidPayment = new Payment
            {
                Description = invalidString
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
            ValueTask<Payment> modifyPaymentTask =
                this.paymentService.ModifyPaymentAsync(invalidPayment);

            PaymentValidationException actualPaymentValidationException =
                await Assert.ThrowsAsync<PaymentValidationException>(
                    modifyPaymentTask.AsTask);

            // then
            actualPaymentValidationException.Should()
                .BeEquivalentTo(expectedPaymentValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePaymentAsync(It.IsAny<Payment>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfPaymentDoesNotExistAndLogItAsync()
        {
            // given
            Payment randomPayment = CreateRandomPayment();
            Payment nonExistPayment = randomPayment;
            Payment nullPayment = null;

            var notFoundPaymentException =
                new NotFoundPaymentException(nonExistPayment.Id);

            var expectedPaymentValidationException =
                new PaymentValidationException(notFoundPaymentException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(nonExistPayment.Id))
                    .ReturnsAsync(nullPayment);

            // when
            ValueTask<Payment> modifyPaymentTask =
                this.paymentService.ModifyPaymentAsync(nonExistPayment);

            PaymentValidationException actualPaymentValidationException =
                await Assert.ThrowsAsync<PaymentValidationException>
                    (modifyPaymentTask.AsTask);

            // then
            actualPaymentValidationException.Should()
                .BeEquivalentTo(expectedPaymentValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(nonExistPayment.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPaymentValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePaymentAsync(nonExistPayment), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
