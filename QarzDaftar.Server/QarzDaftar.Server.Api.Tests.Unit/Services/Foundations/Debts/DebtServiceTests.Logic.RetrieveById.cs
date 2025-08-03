using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveDebtByIdAsync()
        {
            // given
            Guid randomDebtId = Guid.NewGuid();
            Guid inputDebtId = randomDebtId;
            Debt randomDebt = CreateRandomDebt();
            Debt persistedDebt = randomDebt;
            Debt expectedDebt = persistedDebt.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDebtByIdAsync(inputDebtId))
                    .ReturnsAsync(persistedDebt);

            // when
            Debt actualDebt =
                await this.debtService
                    .RetrieveDebtByIdAsync(inputDebtId);

            // then 
            actualDebt.Should().BeEquivalentTo(expectedDebt);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(inputDebtId),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
