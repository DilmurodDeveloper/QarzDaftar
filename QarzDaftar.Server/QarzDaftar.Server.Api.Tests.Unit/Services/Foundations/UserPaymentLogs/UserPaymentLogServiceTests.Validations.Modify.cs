using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionIfUserPaymentLogIsNullAndLogItAsync()
        {
            // given
            UserPaymentLog nullUserPaymentLog = null;
            var nullUserPaymentLogException = new NullUserPaymentLogException();

            var expectedUserPaymentLogValidationException =
                new UserPaymentLogValidationException(nullUserPaymentLogException);

            // when
            ValueTask<UserPaymentLog> modifyUserPaymentLogTask =
                this.userPaymentLogService.ModifyUserPaymentLogAsync(nullUserPaymentLog);

            UserPaymentLogValidationException actualUserPaymentLogValidationException =
                await Assert.ThrowsAsync<UserPaymentLogValidationException>(
                    modifyUserPaymentLogTask.AsTask);

            // then
            actualUserPaymentLogValidationException.Should()
                .BeEquivalentTo(expectedUserPaymentLogValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserPaymentLogValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserPaymentLogAsync(It.IsAny<UserPaymentLog>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserPaymentLogIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            var invalidUserPaymentLog = new UserPaymentLog
            {
                Purpose = invalidString,
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
            ValueTask<UserPaymentLog> modifyUserPaymentLogTask =
                this.userPaymentLogService.ModifyUserPaymentLogAsync(
                    invalidUserPaymentLog);

            UserPaymentLogValidationException actualUserPaymentLogValidationException =
                await Assert.ThrowsAsync<UserPaymentLogValidationException>(
                    modifyUserPaymentLogTask.AsTask);

            // then
            actualUserPaymentLogValidationException.Should()
                .BeEquivalentTo(expectedUserPaymentLogValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserPaymentLogValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserPaymentLogAsync(
                    It.IsAny<UserPaymentLog>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public async Task ShouldThrowValidationExceptionOnModifyIfAmountIsInvalidAndLogItAsync(decimal invalidAmount)
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
            ValueTask<UserPaymentLog> modifyUserPaymentLogTask =
                this.userPaymentLogService.ModifyUserPaymentLogAsync(invalidUserPaymentLog);

            // then
            await Assert.ThrowsAsync<UserPaymentLogValidationException>(() =>
                modifyUserPaymentLogTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedUserPaymentLogValidationException))),
                Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserPaymentLogAsync(It.IsAny<UserPaymentLog>()),
                Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserPaymentLogDoesNotExistAndLogItAsync()
        {
            // given
            UserPaymentLog randomUserPaymentLog = CreateRandomUserPaymentLog();
            UserPaymentLog nonExistUserPaymentLog = randomUserPaymentLog;
            UserPaymentLog nullUserPaymentLog = null;

            var notFoundUserPaymentLogException =
                new NotFoundUserPaymentLogException(nonExistUserPaymentLog.Id);

            var expectedUserPaymentLogValidationException =
                new UserPaymentLogValidationException(notFoundUserPaymentLogException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(
                    nonExistUserPaymentLog.Id)).ReturnsAsync(nullUserPaymentLog);

            // when
            ValueTask<UserPaymentLog> modifyUserPaymentLogTask =
                this.userPaymentLogService.ModifyUserPaymentLogAsync(
                    nonExistUserPaymentLog);

            var actualUserPaymentLogValidationException =
                await Assert.ThrowsAsync<UserPaymentLogValidationException>
                    (modifyUserPaymentLogTask.AsTask);

            // then
            actualUserPaymentLogValidationException.Should()
                .BeEquivalentTo(expectedUserPaymentLogValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(
                    nonExistUserPaymentLog.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserPaymentLogValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserPaymentLogAsync(
                    nonExistUserPaymentLog), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
