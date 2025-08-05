using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllSubscriptionHistorys()
        {
            // given
            IQueryable<SubscriptionHistory>
                randomSubscriptionHistorys =
                    CreateRandomSubscriptionHistories();

            IQueryable<SubscriptionHistory>
                persistedSubscriptionHistorys =
                    randomSubscriptionHistorys;

            IQueryable<SubscriptionHistory>
                expectedSubscriptionHistorys =
                    persistedSubscriptionHistorys.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllSubscriptionHistories())
                    .Returns(persistedSubscriptionHistorys);

            // when
            IQueryable<SubscriptionHistory>
                actualSubscriptionHistorys =
                    this.subscriptionHistoryService
                        .RetrieveAllSubscriptionHistories();

            // then
            actualSubscriptionHistorys.Should()
                .BeEquivalentTo(expectedSubscriptionHistorys);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllSubscriptionHistories(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
