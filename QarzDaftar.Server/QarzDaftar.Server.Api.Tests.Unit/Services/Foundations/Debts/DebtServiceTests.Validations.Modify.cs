using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionIfDebtIsNullAndLogItAsync()
        {
            // given
            Debt nullDebt = null;
            var nullDebtException = new NullDebtException();

            var expectedDebtValidationException =
                new DebtValidationException(nullDebtException);

            // when
            ValueTask<Debt> modifyDebtTask =
                this.debtService.ModifyDebtAsync(nullDebt);

            DebtValidationException actualDebtValidationException =
                await Assert.ThrowsAsync<DebtValidationException>(() =>
                    modifyDebtTask.AsTask());

            // then
            actualDebtValidationException.Should()
                .BeEquivalentTo(expectedDebtValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDebtAsync(It.IsAny<Debt>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
