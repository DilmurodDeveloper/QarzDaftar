using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given 
            Guid invalidSubscriptionHistoryId = Guid.Empty;
            var invalidSubscriptionHistoryException = new InvalidSubscriptionHistoryException();

            invalidSubscriptionHistoryException.AddData(
                key: nameof(SubscriptionHistory.Id),
                values: "Id is required");

            var expectedSubscriptionHistoryValidationException =
                new SubscriptionHistoryValidationException(invalidSubscriptionHistoryException);

            // when
            ValueTask<SubscriptionHistory> removeSubscriptionHistoryById =
                this.subscriptionHistoryService.RemoveSubscriptionHistoryByIdAsync(invalidSubscriptionHistoryId);

            SubscriptionHistoryValidationException actualSubscriptionHistoryValidationException =
                await Assert.ThrowsAsync<SubscriptionHistoryValidationException>(
                    removeSubscriptionHistoryById.AsTask);
            // then
            actualSubscriptionHistoryValidationException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()), Times.Never);

            this.storageBrokerMock.Verify(broker =>
            broker.DeleteSubscriptionHistoryAsync(It.IsAny<SubscriptionHistory>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
