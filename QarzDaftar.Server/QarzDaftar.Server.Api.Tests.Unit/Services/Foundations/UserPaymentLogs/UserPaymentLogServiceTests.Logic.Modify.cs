using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogServiceTests
    {
        [Fact]
        public async Task ShouldModifyUserPaymentLogAsync()
        {
            // given 
            UserPaymentLog randomUserPaymentLog = CreateRandomUserPaymentLog();
            UserPaymentLog inputUserPaymentLog = randomUserPaymentLog;
            UserPaymentLog persistedUserPaymentLog = inputUserPaymentLog.DeepClone();
            UserPaymentLog updatedUserPaymentLog = inputUserPaymentLog;
            UserPaymentLog expectedUserPaymentLog = updatedUserPaymentLog.DeepClone();
            Guid InputUserPaymentLogId = inputUserPaymentLog.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(InputUserPaymentLogId))
                    .ReturnsAsync(persistedUserPaymentLog);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateUserPaymentLogAsync(inputUserPaymentLog))
                    .ReturnsAsync(updatedUserPaymentLog);

            // when
            UserPaymentLog actualUserPaymentLog =
                await this.userPaymentLogService.ModifyUserPaymentLogAsync(inputUserPaymentLog);

            // then
            actualUserPaymentLog.Should().BeEquivalentTo(expectedUserPaymentLog);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(InputUserPaymentLogId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserPaymentLogAsync(inputUserPaymentLog), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
