using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Payments;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Payments
{
    public partial class PaymentServiceTests
    {
        [Fact]
        public async Task ShouldAddPaymentAsync()
        {
            // given
            Payment randomPayment = CreateRandomPayment();
            Payment inputPayment = randomPayment;
            Payment persistedPayment = inputPayment;
            Payment expectedPayment = persistedPayment.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertPaymentAsync(inputPayment))
                    .ReturnsAsync(persistedPayment);

            // when
            Payment actualPayment =
                await this.paymentService.AddPaymentAsync(inputPayment);

            // then
            actualPayment.Should().BeEquivalentTo(expectedPayment);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPaymentAsync(inputPayment), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
