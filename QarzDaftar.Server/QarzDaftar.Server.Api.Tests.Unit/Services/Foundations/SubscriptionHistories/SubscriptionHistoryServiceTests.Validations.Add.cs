using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            SubscriptionHistory nullSubscriptionHistory = null;
            
            var nullSubscriptionHistoryException = 
                new NullSubscriptionHistoryException();

            var expectedSubscriptionHistoryValidationException =
                new SubscriptionHistoryValidationException(nullSubscriptionHistoryException);

            // when
            ValueTask<SubscriptionHistory> addSubscriptionHistoryTask =
                this.subscriptionHistoryService.AddSubscriptionHistoryAsync(
                    nullSubscriptionHistory);

            SubscriptionHistoryValidationException actualSubscriptionHistoryException =
                await Assert.ThrowsAsync<SubscriptionHistoryValidationException>(
                    addSubscriptionHistoryTask.AsTask);

            // then
            actualSubscriptionHistoryException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubscriptionHistoryAsync(It.IsAny<SubscriptionHistory>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ShouldThrowValidationExceptionOnAddIfSubscriptionHistoryIsInvalidAndLogItAsync(
            decimal invalidAmount)
        {
            // given
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
            ValueTask<SubscriptionHistory> addSubscriptionHistoryTask =
                this.subscriptionHistoryService.AddSubscriptionHistoryAsync(invalidSubscriptionHistory);

            SubscriptionHistoryValidationException actualSubscriptionHistoryValidationException =
                await Assert.ThrowsAsync<SubscriptionHistoryValidationException>(
                    addSubscriptionHistoryTask.AsTask);

            // then
            actualSubscriptionHistoryValidationException.Should()
                .BeEquivalentTo(expectedSubscriptionHistoryValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedSubscriptionHistoryValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertSubscriptionHistoryAsync(It.IsAny<SubscriptionHistory>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
