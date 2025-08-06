using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveUserPaymentLogByIdAsync()
        {
            // given
            Guid randomUserPaymentLogId = Guid.NewGuid();
            Guid inputUserPaymentLogId = randomUserPaymentLogId;

            UserPaymentLog randomUserPaymentLog =
                CreateRandomUserPaymentLog();

            UserPaymentLog persistedUserPaymentLog =
                randomUserPaymentLog;

            UserPaymentLog expectedUserPaymentLog =
                persistedUserPaymentLog.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(
                    inputUserPaymentLogId)).ReturnsAsync(
                        persistedUserPaymentLog);

            // when
            UserPaymentLog actualUserPaymentLog =
                await this.userPaymentLogService
                    .RetrieveUserPaymentLogByIdAsync(inputUserPaymentLogId);

            // then 
            actualUserPaymentLog.Should()
                .BeEquivalentTo(expectedUserPaymentLog);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(
                    inputUserPaymentLogId), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
