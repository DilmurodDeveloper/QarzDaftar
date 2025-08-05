using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryServiceTests
    {
        [Fact]
        public async Task ShouldAddSubscriptionHistoryAsync()
        {
            // given
            SubscriptionHistory randomSubscriptionHistory = CreateRandomSubscriptionHistory();
            SubscriptionHistory inputSubscriptionHistory = randomSubscriptionHistory;
            SubscriptionHistory persistedSubscriptionHistory = inputSubscriptionHistory;
            SubscriptionHistory expectedSubscriptionHistory = persistedSubscriptionHistory.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertSubscriptionHistoryAsync(inputSubscriptionHistory))
                    .ReturnsAsync(persistedSubscriptionHistory);

            // when
            SubscriptionHistory actualSubscriptionHistory =
                await this.subscriptionHistoryService.AddSubscriptionHistoryAsync(inputSubscriptionHistory);

            // then
            actualSubscriptionHistory.Should().BeEquivalentTo(expectedSubscriptionHistory);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubscriptionHistoryAsync(inputSubscriptionHistory), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
