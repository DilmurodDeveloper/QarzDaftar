using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllDebts()
        {
            // given
            IQueryable<Debt> randomDebts = CreateRandomDebts();
            IQueryable<Debt> persistedDebts = randomDebts;
            IQueryable<Debt> expectedDebts = persistedDebts.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllDebts()).Returns(persistedDebts);

            // when
            IQueryable<Debt> actualDebts =
                this.debtService.RetrieveAllDebts();

            // then
            actualDebts.Should().BeEquivalentTo(expectedDebts);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllDebts(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
