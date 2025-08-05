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
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someSubscriptionHistoryId = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedSubscriptionHistoryException =
                new LockedSubscriptionHistoryException(dbUpdateConcurrencyException);

            var expectedSubscriptionHistoryDependencyValidationException =
                new SubscriptionHistoryDependencyValidationException(lockedSubscriptionHistoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<SubscriptionHistory> removeSubscriptionHistoryById =
                this.subscriptionHistoryService.RemoveSubscriptionHistoryByIdAsync(someSubscriptionHistoryId);

            var actualSubscriptionHistoryDependencyValidationException =
                await Assert.ThrowsAsync<SubscriptionHistoryDependencyValidationException>(
                    removeSubscriptionHistoryById.AsTask);

            // then
            actualSubscriptionHistoryDependencyValidationException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteSubscriptionHistoryAsync(It.IsAny<SubscriptionHistory>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someSubscriptionHistoryId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedSubscriptionHistoryStorageException =
                new FailedSubscriptionHistoryStorageException(sqlException);

            var expectedSubscriptionHistoryDependencyException =
                new SubscriptionHistoryDependencyException(failedSubscriptionHistoryStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);
            // when
            ValueTask<SubscriptionHistory> deleteSubscriptionHistoryTask =
                this.subscriptionHistoryService.RemoveSubscriptionHistoryByIdAsync(someSubscriptionHistoryId);

            SubscriptionHistoryDependencyException actualSubscriptionHistoryDependencyException =
                await Assert.ThrowsAsync<SubscriptionHistoryDependencyException>(
                    deleteSubscriptionHistoryTask.AsTask);

            // then
            actualSubscriptionHistoryDependencyException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someSubscriptionHistoryId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedSubscriptionHistoryServiceException =
                new FailedSubscriptionHistoryServiceException(serviceException);

            var expectedSubscriptionHistoryServiceException =
                new SubscriptionHistoryServiceException(failedSubscriptionHistoryServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<SubscriptionHistory> removeSubscriptionHistoryByIdTask =
                this.subscriptionHistoryService.RemoveSubscriptionHistoryByIdAsync(someSubscriptionHistoryId);

            SubscriptionHistoryServiceException actualSubscriptionHistoryServiceException =
                await Assert.ThrowsAsync<SubscriptionHistoryServiceException>(
                    removeSubscriptionHistoryByIdTask.AsTask);

            // then
            actualSubscriptionHistoryServiceException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
