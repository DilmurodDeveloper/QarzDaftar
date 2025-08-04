using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Payments;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Payments
{
    public partial class PaymentServiceTests
    {
        [Fact]
        public async Task ShouldRetrievePaymentByIdAsync()
        {
            // given
            Guid randomPaymentId = Guid.NewGuid();
            Guid inputPaymentId = randomPaymentId;
            Payment randomPayment = CreateRandomPayment();
            Payment persistedPayment = randomPayment;
            Payment expectedPayment = persistedPayment.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(inputPaymentId))
                    .ReturnsAsync(persistedPayment);

            // when
            Payment actualPayment =
                await this.paymentService
                    .RetrievePaymentByIdAsync(inputPaymentId);

            // then 
            actualPayment.Should().BeEquivalentTo(expectedPayment);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(inputPaymentId),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
