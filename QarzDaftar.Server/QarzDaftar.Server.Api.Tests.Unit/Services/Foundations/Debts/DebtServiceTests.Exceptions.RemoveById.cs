using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someDebtId = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedDebtException =
                new LockedDebtException(dbUpdateConcurrencyException);

            var expectedDebtDependencyValidationException =
                new DebtDependencyValidationException(lockedDebtException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<Debt> removeDebtById =
                this.debtService.RemoveDebtByIdAsync(someDebtId);

            var actualDebtDependencyValidationException =
                await Assert.ThrowsAsync<DebtDependencyValidationException>(
                    removeDebtById.AsTask);

            // then
            actualDebtDependencyValidationException.Should()
                .BeEquivalentTo(expectedDebtDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteDebtAsync(It.IsAny<Debt>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
