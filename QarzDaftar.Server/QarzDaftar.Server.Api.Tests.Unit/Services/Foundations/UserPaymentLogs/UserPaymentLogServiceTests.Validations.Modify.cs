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
    }
}
