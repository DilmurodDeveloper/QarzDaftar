using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldRemoveDebtByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputDebtId = randomId;
            Debt randomDebt = CreateRandomDebt();
            Debt storageDebt = randomDebt;
            Debt expectedInputDebt = storageDebt;
            Debt deletedDebt = expectedInputDebt;
            Debt expectedDebt = deletedDebt.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDebtByIdAsync(inputDebtId))
                    .ReturnsAsync(storageDebt);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteDebtAsync(expectedInputDebt))
                    .ReturnsAsync(deletedDebt);

            // when
            Debt actualDebt =
                await this.debtService.RemoveDebtByIdAsync(randomId);

            // then 
            actualDebt.Should().BeEquivalentTo(expectedDebt);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(inputDebtId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteDebtAsync(expectedInputDebt),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
