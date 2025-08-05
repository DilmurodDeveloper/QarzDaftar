using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionIfSubscriptionHistoryIsNullAndLogItAsync()
        {
            // given
            SubscriptionHistory nullSubscriptionHistory = null;
            var nullSubscriptionHistoryException = new NullSubscriptionHistoryException();

            var expectedSubscriptionHistoryValidationException =
                new SubscriptionHistoryValidationException(nullSubscriptionHistoryException);

            // when
            ValueTask<SubscriptionHistory> modifySubscriptionHistoryTask =
                this.subscriptionHistoryService.ModifySubscriptionHistoryAsync(nullSubscriptionHistory);

            SubscriptionHistoryValidationException actualSubscriptionHistoryValidationException =
                await Assert.ThrowsAsync<SubscriptionHistoryValidationException>(
                    modifySubscriptionHistoryTask.AsTask);

            // then
            actualSubscriptionHistoryValidationException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriptionHistoryAsync(It.IsAny<SubscriptionHistory>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
