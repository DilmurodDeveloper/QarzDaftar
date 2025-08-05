using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            SubscriptionHistory randomSubscriptionHistory = CreateRandomSubscriptionHistory();
            SubscriptionHistory someSubscriptionHistory = randomSubscriptionHistory;
            Guid SubscriptionHistoryId = someSubscriptionHistory.Id;
            SqlException sqlException = GetSqlError();

            var failedSubscriptionHistoryStorageException =
                new FailedSubscriptionHistoryStorageException(sqlException);

            var expectedSubscriptionHistoryDependencyException =
                new SubscriptionHistoryDependencyException(
                    failedSubscriptionHistoryStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(
                    SubscriptionHistoryId)).Throws(sqlException);

            // when
            ValueTask<SubscriptionHistory> modifySubscriptionHistoryTask =
                this.subscriptionHistoryService.ModifySubscriptionHistoryAsync(
                    someSubscriptionHistory);

            SubscriptionHistoryDependencyException actualSubscriptionHistoryDependencyException =
                 await Assert.ThrowsAsync<SubscriptionHistoryDependencyException>(
                    modifySubscriptionHistoryTask.AsTask);

            // then
            actualSubscriptionHistoryDependencyException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(SubscriptionHistoryId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriptionHistoryAsync(someSubscriptionHistory),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            SubscriptionHistory randomSubscriptionHistory = CreateRandomSubscriptionHistory();
            SubscriptionHistory someSubscriptionHistory = randomSubscriptionHistory;
            Guid subscriptionHistoryId = someSubscriptionHistory.Id;
            var databaseUpdateException = new DbUpdateException();

            var failedSubscriptionHistoryStorageException =
                new FailedSubscriptionHistoryStorageException(databaseUpdateException);

            var expectedSubscriptionHistoryDependencyException =
                new SubscriptionHistoryDependencyException(
                    failedSubscriptionHistoryStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(subscriptionHistoryId))
                    .Throws(databaseUpdateException);

            // when
            ValueTask<SubscriptionHistory> modifySubscriptionHistoryTask =
                this.subscriptionHistoryService.ModifySubscriptionHistoryAsync(
                    someSubscriptionHistory);

            SubscriptionHistoryDependencyException actualSubscriptionHistoryDependencyException =
                 await Assert.ThrowsAsync<SubscriptionHistoryDependencyException>(
                    modifySubscriptionHistoryTask.AsTask);

            // then
            actualSubscriptionHistoryDependencyException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(
                    subscriptionHistoryId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriptionHistoryAsync(someSubscriptionHistory),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            SubscriptionHistory randomSubscriptionHistory = CreateRandomSubscriptionHistory();
            SubscriptionHistory someSubscriptionHistory = randomSubscriptionHistory;
            Guid SubscriptionHistoryId = someSubscriptionHistory.Id;
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedSubscriptionHistoryException =
                new LockedSubscriptionHistoryException(dbUpdateConcurrencyException);

            var expectedSubscriptionHistoryDependencyValidationException =
                new SubscriptionHistoryDependencyValidationException(lockedSubscriptionHistoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(SubscriptionHistoryId))
                    .Throws(dbUpdateConcurrencyException);

            // when
            ValueTask<SubscriptionHistory> modifySubscriptionHistoryTask =
                this.subscriptionHistoryService.ModifySubscriptionHistoryAsync(someSubscriptionHistory);

            SubscriptionHistoryDependencyValidationException actualSubscriptionHistoryDependencyValidationException =
                 await Assert.ThrowsAsync<SubscriptionHistoryDependencyValidationException>(
                    modifySubscriptionHistoryTask.AsTask);

            // then
            actualSubscriptionHistoryDependencyValidationException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(SubscriptionHistoryId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriptionHistoryAsync(someSubscriptionHistory), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            SubscriptionHistory randomSubscriptionHistory = CreateRandomSubscriptionHistory();
            SubscriptionHistory someSubscriptionHistory = randomSubscriptionHistory;
            Guid SubscriptionHistoryId = someSubscriptionHistory.Id;
            var serviceException = new Exception();

            var failedSubscriptionHistoryServiceException =
                new FailedSubscriptionHistoryServiceException(serviceException);

            var expectedSubscriptionHistoryServiceException =
                new SubscriptionHistoryServiceException(failedSubscriptionHistoryServiceException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectSubscriptionHistoryByIdAsync(SubscriptionHistoryId))
                .Throws(serviceException);

            // when
            ValueTask<SubscriptionHistory> modifySubscriptionHistoryTask =
                this.subscriptionHistoryService.ModifySubscriptionHistoryAsync(someSubscriptionHistory);

            SubscriptionHistoryServiceException actualSubscriptionHistoryServiceException =
                await Assert.ThrowsAsync<SubscriptionHistoryServiceException>(
                    modifySubscriptionHistoryTask.AsTask);

            // then
            actualSubscriptionHistoryServiceException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryServiceException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryServiceException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(SubscriptionHistoryId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriptionHistoryAsync(someSubscriptionHistory), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
