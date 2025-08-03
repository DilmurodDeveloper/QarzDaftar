using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldAddDebtAsync()
        {
            // given
            Debt randomDebt = CreateRandomDebt();
            Debt inputDebt = randomDebt;
            Debt persistedDebt = inputDebt;
            Debt expectedDebt = persistedDebt.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertDebtAsync(inputDebt))
                    .ReturnsAsync(persistedDebt);

            // when
            Debt actualDebt =
                await this.debtService.AddDebtAsync(inputDebt);

            // then
            actualDebt.Should().BeEquivalentTo(expectedDebt);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDebtAsync(inputDebt), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
