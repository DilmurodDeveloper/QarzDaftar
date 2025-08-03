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

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someDebtId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedDebtStorageException =
                new FailedDebtStorageException(sqlException);

            var expectedDebtDependencyException =
                new DebtDependencyException(failedDebtStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);
            // when
            ValueTask<Debt> deleteDebtTask =
                this.debtService.RemoveDebtByIdAsync(someDebtId);

            DebtDependencyException actualDebtDependencyException =
                await Assert.ThrowsAsync<DebtDependencyException>(
                    deleteDebtTask.AsTask);

            // then
            actualDebtDependencyException.Should()
                .BeEquivalentTo(expectedDebtDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedDebtDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someDebtId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedDebtServiceException =
                new FailedDebtServiceException(serviceException);

            var expectedDebtServiceException =
                new DebtServiceException(failedDebtServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Debt> removeDebtByIdTask =
                this.debtService.RemoveDebtByIdAsync(someDebtId);

            DebtServiceException actualDebtServiceException =
                await Assert.ThrowsAsync<DebtServiceException>(
                    removeDebtByIdTask.AsTask);

            // then
            actualDebtServiceException.Should()
                .BeEquivalentTo(expectedDebtServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectDebtByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedDebtServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
