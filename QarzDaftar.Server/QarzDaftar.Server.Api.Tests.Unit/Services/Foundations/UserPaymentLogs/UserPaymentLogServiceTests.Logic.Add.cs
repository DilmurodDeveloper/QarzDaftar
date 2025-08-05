using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogServiceTests
    {
        [Fact]
        public async Task ShouldAddUserPaymentLogAsync()
        {
            // given
            UserPaymentLog randomUserPaymentLog = CreateRandomUserPaymentLog();
            UserPaymentLog inputUserPaymentLog = randomUserPaymentLog;
            UserPaymentLog persistedUserPaymentLog = inputUserPaymentLog;
            UserPaymentLog expectedUserPaymentLog = persistedUserPaymentLog.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertUserPaymentLogAsync(inputUserPaymentLog))
                    .ReturnsAsync(persistedUserPaymentLog);

            // when
            UserPaymentLog actualUserPaymentLog =
                await this.userPaymentLogService.AddUserPaymentLogAsync(inputUserPaymentLog);

            // then
            actualUserPaymentLog.Should().BeEquivalentTo(expectedUserPaymentLog);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserPaymentLogAsync(inputUserPaymentLog), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
