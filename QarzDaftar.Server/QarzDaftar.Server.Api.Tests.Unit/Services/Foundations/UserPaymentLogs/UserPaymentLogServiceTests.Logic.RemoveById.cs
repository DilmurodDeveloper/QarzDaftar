using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogServiceTests
    {
        [Fact]
        public async Task ShouldRemoveUserPaymentLogByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputUserPaymentLogId = randomId;

            UserPaymentLog randomUserPaymentLog =
                CreateRandomUserPaymentLog();

            UserPaymentLog storageUserPaymentLog =
                randomUserPaymentLog;

            UserPaymentLog expectedInputUserPaymentLog =
                storageUserPaymentLog;

            UserPaymentLog deletedUserPaymentLog =
                expectedInputUserPaymentLog;

            UserPaymentLog expectedUserPaymentLog =
                deletedUserPaymentLog.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(inputUserPaymentLogId))
                    .ReturnsAsync(storageUserPaymentLog);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteUserPaymentLogAsync(expectedInputUserPaymentLog))
                    .ReturnsAsync(deletedUserPaymentLog);

            // when
            UserPaymentLog actualUserPaymentLog =
                await this.userPaymentLogService
                    .RemoveUserPaymentLogByIdAsync(randomId);

            // then 
            actualUserPaymentLog.Should()
                .BeEquivalentTo(expectedUserPaymentLog);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(
                    inputUserPaymentLogId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteUserPaymentLogAsync(
                        expectedInputUserPaymentLog), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
