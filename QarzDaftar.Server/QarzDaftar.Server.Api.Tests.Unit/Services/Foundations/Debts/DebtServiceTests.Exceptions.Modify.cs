using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Debts
{
    public partial class DebtServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Debt randomDebt = CreateRandomDebt();
            Debt someDebt = randomDebt;
            Guid DebtId = someDebt.Id;
            SqlException sqlException = GetSqlError();

            var failedDebtStorageException =
                new FailedDebtStorageException(sqlException);

            var expectedDebtDependencyException =
                new DebtDependencyException(failedDebtStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDebtByIdAsync(DebtId)).Throws(sqlException);

            // when
            ValueTask<Debt> modifyDebtTask =
                this.debtService.ModifyDebtAsync(someDebt);

            DebtDependencyException actualDebtDependencyException =
                 await Assert.ThrowsAsync<DebtDependencyException>(
                    modifyDebtTask.AsTask);

            // then
            actualDebtDependencyException.Should()
                .BeEquivalentTo(expectedDebtDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedDebtDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(DebtId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDebtAsync(someDebt), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            Debt randomDebt = CreateRandomDebt();
            Debt someDebt = randomDebt;
            Guid debtId = someDebt.Id;
            var databaseUpdateException = new DbUpdateException();

            var failedDebtStorageException =
                new FailedDebtStorageException(databaseUpdateException);

            var expectedDebtDependencyException =
                new DebtDependencyException(failedDebtStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDebtByIdAsync(debtId))
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Debt> modifyDebtTask =
                this.debtService.ModifyDebtAsync(someDebt);

            DebtDependencyException actualDebtDependencyException =
                 await Assert.ThrowsAsync<DebtDependencyException>(
                    modifyDebtTask.AsTask);

            // then
            actualDebtDependencyException.Should()
                .BeEquivalentTo(expectedDebtDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(debtId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateDebtAsync(someDebt), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
