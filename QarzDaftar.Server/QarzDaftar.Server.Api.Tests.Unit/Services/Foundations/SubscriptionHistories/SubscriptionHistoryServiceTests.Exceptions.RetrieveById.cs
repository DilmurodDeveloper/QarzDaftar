using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowSqlExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedSubscriptionHistoryStorageException =
                new FailedSubscriptionHistoryStorageException(sqlException);

            var expectedSubscriptionHistoryDependencyException =
                new SubscriptionHistoryDependencyException(
                    failedSubscriptionHistoryStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<SubscriptionHistory> retrieveSubscriptionHistoryByIdTask =
                this.subscriptionHistoryService.RetrieveSubscriptionHistoryByIdAsync(someId);

            SubscriptionHistoryDependencyException actualSubscriptionHistoryDependencyException =
                await Assert.ThrowsAsync<SubscriptionHistoryDependencyException>(
                    retrieveSubscriptionHistoryByIdTask.AsTask);

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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedSubscriptionHistoryServiceException =
                new FailedSubscriptionHistoryServiceException(serviceException);

            var expectedSubscriptionHistoryServiceException =
                new SubscriptionHistoryServiceException(failedSubscriptionHistoryServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<SubscriptionHistory> retrieveSubscriptionHistoryByIdTask =
                this.subscriptionHistoryService.RetrieveSubscriptionHistoryByIdAsync(someId);

            SubscriptionHistoryServiceException actualSubscriptionHistoryServiceException =
                await Assert.ThrowsAsync<SubscriptionHistoryServiceException>(
                    retrieveSubscriptionHistoryByIdTask.AsTask);

            // then
            actualSubscriptionHistoryServiceException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

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
