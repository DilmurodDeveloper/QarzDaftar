using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            Debt nullDebt = null;
            var nullDebtException = new NullDebtException();

            var expectedDebtValidationException =
                new DebtValidationException(nullDebtException);

            // when
            ValueTask<Debt> addDebtTask =
                this.debtService.AddDebtAsync(nullDebt);

            DebtValidationException actualDebtException =
                await Assert.ThrowsAsync<DebtValidationException>(
                    addDebtTask.AsTask);

            // then
            actualDebtException.Should()
                .BeEquivalentTo(expectedDebtValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertDebtAsync(It.IsAny<Debt>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
