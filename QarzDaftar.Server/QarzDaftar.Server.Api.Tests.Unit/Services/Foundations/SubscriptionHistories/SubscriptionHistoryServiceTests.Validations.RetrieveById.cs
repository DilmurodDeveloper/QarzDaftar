using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
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
            ValueTask<SubscriptionHistory> retrieveSubscriptionHistoryById =
                this.subscriptionHistoryService.RetrieveSubscriptionHistoryByIdAsync(
                    invalidSubscriptionHistoryId);

            var actualSubscriptionHistoryValidationException =
                await Assert.ThrowsAsync<SubscriptionHistoryValidationException>(
                    retrieveSubscriptionHistoryById.AsTask);

            // then
            actualSubscriptionHistoryValidationException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfSubscriptionHistoryNotFoundAndLogItAsync()
        {
            // given
            Guid someSubscriptionHistoryId = Guid.NewGuid();
            SubscriptionHistory noSubscriptionHistory = null;

            var notFoundSubscriptionHistoryException =
                new NotFoundSubscriptionHistoryException(someSubscriptionHistoryId);

            var expetedSubscriptionHistoryValidationException =
                new SubscriptionHistoryValidationException(notFoundSubscriptionHistoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noSubscriptionHistory);

            // when
            ValueTask<SubscriptionHistory> retriveByIdSubscriptionHistoryTask =
                this.subscriptionHistoryService.RetrieveSubscriptionHistoryByIdAsync(
                    someSubscriptionHistoryId);

            var actualSubscriptionHistoryValidationException =
                await Assert.ThrowsAsync<SubscriptionHistoryValidationException>(
                    retriveByIdSubscriptionHistoryTask.AsTask);

            // then
            actualSubscriptionHistoryValidationException.Should()
                .BeEquivalentTo(expetedSubscriptionHistoryValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(someSubscriptionHistoryId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expetedSubscriptionHistoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
