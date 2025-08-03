using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldModifyDebtAsync()
        {
            // given 
            Debt randomDebt = CreateRandomDebt();
            Debt inputDebt = randomDebt;
            Debt persistedDebt = inputDebt.DeepClone();
            Debt updatedDebt = inputDebt;
            Debt expectedDebt = updatedDebt.DeepClone();
            Guid InputDebtId = inputDebt.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDebtByIdAsync(InputDebtId))
                    .ReturnsAsync(persistedDebt);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateDebtAsync(inputDebt))
                    .ReturnsAsync(updatedDebt);

            // when
            Debt actualDebt =
                await this.debtService.ModifyDebtAsync(inputDebt);

            // then
            actualDebt.Should().BeEquivalentTo(expectedDebt);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(InputDebtId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDebtAsync(inputDebt), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
