using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryServiceTests
    {
        [Fact]
        public async Task ShouldRemoveSubscriptionHistoryByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputSubscriptionHistoryId = randomId;
            SubscriptionHistory randomSubscriptionHistory = CreateRandomSubscriptionHistory();
            SubscriptionHistory storageSubscriptionHistory = randomSubscriptionHistory;
            SubscriptionHistory expectedInputSubscriptionHistory = storageSubscriptionHistory;
            SubscriptionHistory deletedSubscriptionHistory = expectedInputSubscriptionHistory;
            SubscriptionHistory expectedSubscriptionHistory = deletedSubscriptionHistory.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(inputSubscriptionHistoryId))
                    .ReturnsAsync(storageSubscriptionHistory);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteSubscriptionHistoryAsync(expectedInputSubscriptionHistory))
                    .ReturnsAsync(deletedSubscriptionHistory);

            // when
            SubscriptionHistory actualSubscriptionHistory =
                await this.subscriptionHistoryService.RemoveSubscriptionHistoryByIdAsync(randomId);

            // then 
            actualSubscriptionHistory.Should().BeEquivalentTo(expectedSubscriptionHistory);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(inputSubscriptionHistoryId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteSubscriptionHistoryAsync(expectedInputSubscriptionHistory),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
