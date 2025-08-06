using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            UserPaymentLog nullUserPaymentLog = null;
            var nullUserPaymentLogException = new NullUserPaymentLogException();

            var expectedUserPaymentLogValidationException =
                new UserPaymentLogValidationException(nullUserPaymentLogException);

            // when
            ValueTask<UserPaymentLog> addUserPaymentLogTask =
                this.userPaymentLogService.AddUserPaymentLogAsync(nullUserPaymentLog);

            UserPaymentLogValidationException actualUserPaymentLogException =
                await Assert.ThrowsAsync<UserPaymentLogValidationException>(
                    addUserPaymentLogTask.AsTask);

            // then
            actualUserPaymentLogException.Should()
                .BeEquivalentTo(expectedUserPaymentLogValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserPaymentLogValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserPaymentLogAsync(It.IsAny<UserPaymentLog>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfUserPaymentLogIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidUserPaymentLog = new UserPaymentLog
            {
                Purpose = invalidText,
            };

            var invalidUserPaymentLogException = new InvalidUserPaymentLogException();

            invalidUserPaymentLogException.AddData(
                key: nameof(UserPaymentLog.Id),
                values: "Id is required");

            invalidUserPaymentLogException.AddData(
                key: nameof(UserPaymentLog.PaymentMethod),
                values: "Text is required");

            invalidUserPaymentLogException.AddData(
                key: nameof(UserPaymentLog.Purpose),
                values: "Text is required");

            invalidUserPaymentLogException.AddData(
                key: nameof(UserPaymentLog.Comment),
                values: "Text is required");

            invalidUserPaymentLogException.AddData(
                key: nameof(UserPaymentLog.PaidAt),
                values: "Date is required");

            invalidUserPaymentLogException.AddData(
                key: nameof(UserPaymentLog.CreatedDate),
                values: "Date is required");

            invalidUserPaymentLogException.AddData(
                key: nameof(UserPaymentLog.UserId),
                values: "Id is required");

            var expectedUserPaymentLogValidationException =
                new UserPaymentLogValidationException(invalidUserPaymentLogException);

            // when
            ValueTask<UserPaymentLog> addUserPaymentLogTask =
                this.userPaymentLogService.AddUserPaymentLogAsync(invalidUserPaymentLog);

            UserPaymentLogValidationException actualUserPaymentLogValidationException =
                await Assert.ThrowsAsync<UserPaymentLogValidationException>(
                    addUserPaymentLogTask.AsTask);

            // then
            actualUserPaymentLogValidationException.Should()
                .BeEquivalentTo(expectedUserPaymentLogValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserPaymentLogValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserPaymentLogAsync(It.IsAny<UserPaymentLog>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task ShouldThrowValidationExceptionOnAddIfAmountIsInvalidAndLogItAsync(decimal invalidAmount)
        {
            // given
            UserPaymentLog randomUserPaymentLog = CreateRandomUserPaymentLog();
            UserPaymentLog invalidUserPaymentLog = randomUserPaymentLog;
            invalidUserPaymentLog.Amount = invalidAmount;

            var invalidUserPaymentLogException = new InvalidUserPaymentLogException();

            invalidUserPaymentLogException.AddData(
                key: nameof(UserPaymentLog.Amount),
                values: "Amount must be greater than zero");

            var expectedUserPaymentLogValidationException =
                new UserPaymentLogValidationException(invalidUserPaymentLogException);

            // when
            ValueTask<UserPaymentLog> addUserPaymentLogTask =
                this.userPaymentLogService.AddUserPaymentLogAsync(invalidUserPaymentLog);

            // then
            await Assert.ThrowsAsync<UserPaymentLogValidationException>(() =>
                addUserPaymentLogTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedUserPaymentLogValidationException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserPaymentLogAsync(It.IsAny<UserPaymentLog>()),
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
