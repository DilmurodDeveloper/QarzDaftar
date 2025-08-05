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
    }
}
