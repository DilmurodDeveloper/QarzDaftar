using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryServiceTests
    {
        [Fact]
        public async Task ShouldModifySubscriptionHistoryAsync()
        {
            // given 
            SubscriptionHistory randomSubscriptionHistory =
                CreateRandomSubscriptionHistory();

            SubscriptionHistory inputSubscriptionHistory =
                randomSubscriptionHistory;

            SubscriptionHistory persistedSubscriptionHistory =
                inputSubscriptionHistory.DeepClone();

            SubscriptionHistory updatedSubscriptionHistory =
                inputSubscriptionHistory;

            SubscriptionHistory expectedSubscriptionHistory =
                updatedSubscriptionHistory.DeepClone();

            Guid InputSubscriptionHistoryId = inputSubscriptionHistory.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(
                    InputSubscriptionHistoryId)).ReturnsAsync(
                        persistedSubscriptionHistory);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateSubscriptionHistoryAsync(inputSubscriptionHistory))
                    .ReturnsAsync(updatedSubscriptionHistory);

            // when
            var actualSubscriptionHistory = await this.subscriptionHistoryService
                .ModifySubscriptionHistoryAsync(inputSubscriptionHistory);

            // then
            actualSubscriptionHistory.Should()
                .BeEquivalentTo(expectedSubscriptionHistory);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(
                    InputSubscriptionHistoryId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriptionHistoryAsync(
                    inputSubscriptionHistory), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
