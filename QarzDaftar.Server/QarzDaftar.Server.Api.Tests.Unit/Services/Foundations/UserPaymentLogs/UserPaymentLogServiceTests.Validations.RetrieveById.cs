using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidUserPaymentLogId = Guid.Empty;
            var invalidUserPaymentLogException = new InvalidUserPaymentLogException();

            invalidUserPaymentLogException.AddData(
                key: nameof(UserPaymentLog.Id),
                values: "Id is required");

            var expectedUserPaymentLogValidationException =
                new UserPaymentLogValidationException(invalidUserPaymentLogException);

            // when
            ValueTask<UserPaymentLog> retrieveUserPaymentLogById =
                this.userPaymentLogService.RetrieveUserPaymentLogByIdAsync(
                    invalidUserPaymentLogId);

            var actualUserPaymentLogValidationException =
                await Assert.ThrowsAsync<UserPaymentLogValidationException>(
                    retrieveUserPaymentLogById.AsTask);

            // then
            actualUserPaymentLogValidationException.Should()
                .BeEquivalentTo(expectedUserPaymentLogValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserPaymentLogValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfUserPaymentLogNotFoundAndLogItAsync()
        {
            // given
            Guid someUserPaymentLogId = Guid.NewGuid();
            UserPaymentLog noUserPaymentLog = null;

            var notFoundUserPaymentLogException =
                new NotFoundUserPaymentLogException(someUserPaymentLogId);

            var expetedUserPaymentLogValidationException =
                new UserPaymentLogValidationException(notFoundUserPaymentLogException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noUserPaymentLog);

            // when
            ValueTask<UserPaymentLog> retriveByIdUserPaymentLogTask =
                this.userPaymentLogService.RetrieveUserPaymentLogByIdAsync(
                    someUserPaymentLogId);

            var actualUserPaymentLogValidationException =
                await Assert.ThrowsAsync<UserPaymentLogValidationException>(
                    retriveByIdUserPaymentLogTask.AsTask);

            // then
            actualUserPaymentLogValidationException.Should()
                .BeEquivalentTo(expetedUserPaymentLogValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(someUserPaymentLogId),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expetedUserPaymentLogValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
