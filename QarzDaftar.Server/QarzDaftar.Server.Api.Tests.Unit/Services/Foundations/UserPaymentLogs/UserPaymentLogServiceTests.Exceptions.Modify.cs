using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            UserPaymentLog randomUserPaymentLog = CreateRandomUserPaymentLog();
            UserPaymentLog someUserPaymentLog = randomUserPaymentLog;
            Guid UserPaymentLogId = someUserPaymentLog.Id;
            SqlException sqlException = GetSqlError();

            var failedUserPaymentLogStorageException =
                new FailedUserPaymentLogStorageException(sqlException);

            var expectedUserPaymentLogDependencyException =
                new UserPaymentLogDependencyException(
                    failedUserPaymentLogStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(
                    UserPaymentLogId)).Throws(sqlException);

            // when
            ValueTask<UserPaymentLog> modifyUserPaymentLogTask =
                this.userPaymentLogService.ModifyUserPaymentLogAsync(
                    someUserPaymentLog);

            UserPaymentLogDependencyException actualUserPaymentLogDependencyException =
                 await Assert.ThrowsAsync<UserPaymentLogDependencyException>(
                    modifyUserPaymentLogTask.AsTask);

            // then
            actualUserPaymentLogDependencyException.Should()
                .BeEquivalentTo(expectedUserPaymentLogDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserPaymentLogDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(UserPaymentLogId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserPaymentLogAsync(someUserPaymentLog),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
