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

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRemoveSubscriptionHistoryByIdIsNotFoundAndLogItAsync()
        {
            // given
            Guid inputSubscriptionHistoryId = Guid.NewGuid();
            SubscriptionHistory noSubscriptionHistory = null;

            var notFoundSubscriptionHistoryException =
                new NotFoundSubscriptionHistoryException(inputSubscriptionHistoryId);

            var expectedSubscriptionHistoryValidationException =
                new SubscriptionHistoryValidationException(notFoundSubscriptionHistoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noSubscriptionHistory);

            // when
            ValueTask<SubscriptionHistory> removeSubscriptionHistoryById =
                this.subscriptionHistoryService.RemoveSubscriptionHistoryByIdAsync(
                    inputSubscriptionHistoryId);

            var actualSubscriptionHistoryValidationException =
                await Assert.ThrowsAsync<SubscriptionHistoryValidationException>(
                    removeSubscriptionHistoryById.AsTask);

            // then
            actualSubscriptionHistoryValidationException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteSubscriptionHistoryAsync(It.IsAny<SubscriptionHistory>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
