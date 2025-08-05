using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursLogItAsync()
        {
            // given
            SubscriptionHistory someSubscriptionHistory = CreateRandomSubscriptionHistory();
            SqlException sqlException = GetSqlError();

            var failedSubscriptionHistoryStorageException =
                new FailedSubscriptionHistoryStorageException(sqlException);

            var expectedSubscriptionHistoryDependencyException =
                new SubscriptionHistoryDependencyException(
                    failedSubscriptionHistoryStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertSubscriptionHistoryAsync(someSubscriptionHistory))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<SubscriptionHistory> addSubscriptionHistoryTask =
                this.subscriptionHistoryService.AddSubscriptionHistoryAsync(
                    someSubscriptionHistory);

            // then
            await Assert.ThrowsAsync<SubscriptionHistoryDependencyException>(() =>
                addSubscriptionHistoryTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubscriptionHistoryAsync(someSubscriptionHistory),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccursAndLogItAsync()
        {
            // given
            string someMessage = GetRandomString();
            SubscriptionHistory someSubscriptionHistory = CreateRandomSubscriptionHistory();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsSubscriptionHistoryException =
                new AlreadyExistsSubscriptionHistoryException(duplicateKeyException);

            var expectedSubscriptionHistoryDependencyValidationException =
                new SubscriptionHistoryDependencyValidationException(
                    alreadyExistsSubscriptionHistoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertSubscriptionHistoryAsync(someSubscriptionHistory))
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<SubscriptionHistory> addSubscriptionHistoryTask =
                this.subscriptionHistoryService.AddSubscriptionHistoryAsync(
                    someSubscriptionHistory);

            var actualSubscriptionHistoryDependencyValidationException =
                await Assert.ThrowsAsync<SubscriptionHistoryDependencyValidationException>(
                    addSubscriptionHistoryTask.AsTask);

            // then
            actualSubscriptionHistoryDependencyValidationException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubscriptionHistoryAsync(someSubscriptionHistory),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            SubscriptionHistory someSubscriptionHistory = CreateRandomSubscriptionHistory();
            var serviceException = new Exception();

            var failedSubscriptionHistoryServiceException =
                new FailedSubscriptionHistoryServiceException(serviceException);

            var expectedSubscriptionHistoryServiceException =
                new SubscriptionHistoryServiceException(
                    failedSubscriptionHistoryServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertSubscriptionHistoryAsync(someSubscriptionHistory))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<SubscriptionHistory> addSubscriptionHistoryTask =
                this.subscriptionHistoryService.AddSubscriptionHistoryAsync(
                    someSubscriptionHistory);

            var actualSubscriptionHistoryService =
                await Assert.ThrowsAsync<SubscriptionHistoryServiceException>(
                    addSubscriptionHistoryTask.AsTask);

            // then
            actualSubscriptionHistoryService.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubscriptionHistoryAsync(someSubscriptionHistory),
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
