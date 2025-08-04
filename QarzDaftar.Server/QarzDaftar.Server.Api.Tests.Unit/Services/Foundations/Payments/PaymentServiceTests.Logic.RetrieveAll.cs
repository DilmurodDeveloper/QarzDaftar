using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Payments;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Payments
{
    public partial class PaymentServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllPayments()
        {
            // given
            IQueryable<Payment> randomPayments = CreateRandomPayments();
            IQueryable<Payment> persistedPayments = randomPayments;
            IQueryable<Payment> expectedPayments = persistedPayments.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllPayments()).Returns(persistedPayments);

            // when
            IQueryable<Payment> actualPayments =
                this.paymentService.RetrieveAllPayments();

            // then
            actualPayments.Should().BeEquivalentTo(expectedPayments);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllPayments(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
