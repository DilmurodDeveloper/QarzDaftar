using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Payments;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Payments
{
    public partial class PaymentServiceTests
    {
        [Fact]
        public async Task ShouldRemovePaymentByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputPaymentId = randomId;
            Payment randomPayment = CreateRandomPayment();
            Payment storagePayment = randomPayment;
            Payment expectedInputPayment = storagePayment;
            Payment deletedPayment = expectedInputPayment;
            Payment expectedPayment = deletedPayment.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPaymentByIdAsync(inputPaymentId))
                    .ReturnsAsync(storagePayment);

            this.storageBrokerMock.Setup(broker =>
                broker.DeletePaymentAsync(expectedInputPayment))
                    .ReturnsAsync(deletedPayment);

            // when
            Payment actualPayment =
                await this.paymentService.RemovePaymentByIdAsync(randomId);

            // then 
            actualPayment.Should().BeEquivalentTo(expectedPayment);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPaymentByIdAsync(inputPaymentId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeletePaymentAsync(expectedInputPayment),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
