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
    }
}
