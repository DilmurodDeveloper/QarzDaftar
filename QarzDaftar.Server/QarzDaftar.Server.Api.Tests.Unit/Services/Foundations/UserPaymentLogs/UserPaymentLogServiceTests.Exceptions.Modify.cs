using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
                new UserPaymentLogDependencyException(failedUserPaymentLogStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(
                    UserPaymentLogId)).Throws(sqlException);

            // when
            ValueTask<UserPaymentLog> modifyUserPaymentLogTask =
                this.userPaymentLogService.ModifyUserPaymentLogAsync(someUserPaymentLog);

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

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            UserPaymentLog randomUserPaymentLog = CreateRandomUserPaymentLog();
            UserPaymentLog someUserPaymentLog = randomUserPaymentLog;
            Guid userPaymentLogId = someUserPaymentLog.Id;
            var databaseUpdateException = new DbUpdateException();

            var failedUserPaymentLogStorageException =
                new FailedUserPaymentLogStorageException(databaseUpdateException);

            var expectedUserPaymentLogDependencyException =
                new UserPaymentLogDependencyException(failedUserPaymentLogStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(userPaymentLogId))
                    .Throws(databaseUpdateException);

            // when
            ValueTask<UserPaymentLog> modifyUserPaymentLogTask =
                this.userPaymentLogService.ModifyUserPaymentLogAsync(someUserPaymentLog);

            UserPaymentLogDependencyException actualUserPaymentLogDependencyException =
                 await Assert.ThrowsAsync<UserPaymentLogDependencyException>(
                    modifyUserPaymentLogTask.AsTask);

            // then
            actualUserPaymentLogDependencyException.Should()
                .BeEquivalentTo(expectedUserPaymentLogDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserPaymentLogDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(userPaymentLogId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserPaymentLogAsync(someUserPaymentLog), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            UserPaymentLog randomUserPaymentLog = CreateRandomUserPaymentLog();
            UserPaymentLog someUserPaymentLog = randomUserPaymentLog;
            Guid UserPaymentLogId = someUserPaymentLog.Id;
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedUserPaymentLogException =
                new LockedUserPaymentLogException(dbUpdateConcurrencyException);

            var expectedUserPaymentLogDependencyValidationException =
                new UserPaymentLogDependencyValidationException(lockedUserPaymentLogException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(UserPaymentLogId))
                    .Throws(dbUpdateConcurrencyException);

            // when
            ValueTask<UserPaymentLog> modifyUserPaymentLogTask =
                this.userPaymentLogService.ModifyUserPaymentLogAsync(someUserPaymentLog);

            UserPaymentLogDependencyValidationException actualUserPaymentLogDependencyValidationException =
                 await Assert.ThrowsAsync<UserPaymentLogDependencyValidationException>(
                    modifyUserPaymentLogTask.AsTask);

            // then
            actualUserPaymentLogDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserPaymentLogDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserPaymentLogDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(UserPaymentLogId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserPaymentLogAsync(someUserPaymentLog), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
