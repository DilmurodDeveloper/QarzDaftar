using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveSubscriptionHistoryByIdAsync()
        {
            // given
            Guid randomSubscriptionHistoryId = Guid.NewGuid();
            Guid inputSubscriptionHistoryId = randomSubscriptionHistoryId;

            SubscriptionHistory randomSubscriptionHistory =
                CreateRandomSubscriptionHistory();

            SubscriptionHistory persistedSubscriptionHistory =
                randomSubscriptionHistory;

            SubscriptionHistory expectedSubscriptionHistory =
                persistedSubscriptionHistory.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(
                    inputSubscriptionHistoryId)).ReturnsAsync(
                        persistedSubscriptionHistory);

            // when
            SubscriptionHistory actualSubscriptionHistory =
                await this.subscriptionHistoryService
                .RetrieveSubscriptionHistoryByIdAsync(
                    inputSubscriptionHistoryId);

            // then 
            actualSubscriptionHistory.Should()
                .BeEquivalentTo(expectedSubscriptionHistory);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(
                    inputSubscriptionHistoryId), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
