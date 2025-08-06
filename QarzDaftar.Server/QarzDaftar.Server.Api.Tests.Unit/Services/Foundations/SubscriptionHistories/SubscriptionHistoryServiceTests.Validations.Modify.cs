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

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task ShouldThrowValidationExceptionOnModifyIfSubscriptionHistoryIsInvalidAndLogItAsync(
            decimal invalidAmount)
        {
            //given
            var invalidSubscriptionHistory = new SubscriptionHistory
            {
                Amount = invalidAmount,
            };

            var invalidSubscriptionHistoryException = new InvalidSubscriptionHistoryException();

            invalidSubscriptionHistoryException.AddData(
                key: nameof(SubscriptionHistory.Id),
                values: "Id is required");

            invalidSubscriptionHistoryException.AddData(
                key: nameof(SubscriptionHistory.Amount),
                values: "Amount is invalid");

            invalidSubscriptionHistoryException.AddData(
                key: nameof(SubscriptionHistory.PurchasedAt),
                values: "Date is required");

            invalidSubscriptionHistoryException.AddData(
                key: nameof(SubscriptionHistory.ExpiresAt),
                values: "Date is required");

            invalidSubscriptionHistoryException.AddData(
                key: nameof(SubscriptionHistory.UserId),
                values: "Id is required");

            var expectedSubscriptionHistoryValidationException =
                new SubscriptionHistoryValidationException(invalidSubscriptionHistoryException);

            // when
            ValueTask<SubscriptionHistory> modifySubscriptionHistoryTask =
                this.subscriptionHistoryService.ModifySubscriptionHistoryAsync(
                    invalidSubscriptionHistory);

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
                broker.UpdateSubscriptionHistoryAsync(
                    It.IsAny<SubscriptionHistory>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfSubscriptionHistoryDoesNotExistAndLogItAsync()
        {
            // given
            SubscriptionHistory randomSubscriptionHistory = CreateRandomSubscriptionHistory();
            SubscriptionHistory nonExistSubscriptionHistory = randomSubscriptionHistory;
            SubscriptionHistory nullSubscriptionHistory = null;

            var notFoundSubscriptionHistoryException =
                new NotFoundSubscriptionHistoryException(nonExistSubscriptionHistory.Id);

            var expectedSubscriptionHistoryValidationException =
                new SubscriptionHistoryValidationException(notFoundSubscriptionHistoryException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(
                    nonExistSubscriptionHistory.Id)).ReturnsAsync(nullSubscriptionHistory);

            // when
            ValueTask<SubscriptionHistory> modifySubscriptionHistoryTask =
                this.subscriptionHistoryService.ModifySubscriptionHistoryAsync(
                    nonExistSubscriptionHistory);

            var actualSubscriptionHistoryValidationException =
                await Assert.ThrowsAsync<SubscriptionHistoryValidationException>
                    (modifySubscriptionHistoryTask.AsTask);

            // then
            actualSubscriptionHistoryValidationException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSubscriptionHistoryByIdAsync(
                    nonExistSubscriptionHistory.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateSubscriptionHistoryAsync(
                    nonExistSubscriptionHistory), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
