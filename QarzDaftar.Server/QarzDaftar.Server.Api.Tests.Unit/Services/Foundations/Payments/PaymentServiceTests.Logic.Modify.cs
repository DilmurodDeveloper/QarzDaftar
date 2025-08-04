using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Payments;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Payments
{
    public partial class PaymentServiceTests
    {
        [Fact]
        public async Task ShouldModifyPaymentAsync()
        {
            // given 
            Payment randomPayment = CreateRandomPayment();
            Payment inputPayment = randomPayment;
            Payment persistedPayment = inputPayment.DeepClone();
            Payment updatedPayment = inputPayment;
            Payment expectedPayment = updatedPayment.DeepClone();
            Guid InputPaymentId = inputPayment.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(InputPaymentId))
                    .ReturnsAsync(persistedPayment);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdatePaymentAsync(inputPayment))
                    .ReturnsAsync(updatedPayment);

            // when
            Payment actualPayment =
                await this.paymentService.ModifyPaymentAsync(inputPayment);

            // then
            actualPayment.Should().BeEquivalentTo(expectedPayment);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(InputPaymentId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePaymentAsync(inputPayment), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
