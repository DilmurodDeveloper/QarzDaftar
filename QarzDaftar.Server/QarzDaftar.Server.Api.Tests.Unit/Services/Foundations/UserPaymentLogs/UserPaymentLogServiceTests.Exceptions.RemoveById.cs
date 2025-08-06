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
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someUserPaymentLogId = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedUserPaymentLogException =
                new LockedUserPaymentLogException(dbUpdateConcurrencyException);

            var expectedUserPaymentLogDependencyValidationException =
                new UserPaymentLogDependencyValidationException(lockedUserPaymentLogException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<UserPaymentLog> removeUserPaymentLogById =
                this.userPaymentLogService.RemoveUserPaymentLogByIdAsync(someUserPaymentLogId);

            var actualUserPaymentLogDependencyValidationException =
                await Assert.ThrowsAsync<UserPaymentLogDependencyValidationException>(
                    removeUserPaymentLogById.AsTask);

            // then
            actualUserPaymentLogDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserPaymentLogDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserPaymentLogDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteUserPaymentLogAsync(It.IsAny<UserPaymentLog>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someUserPaymentLogId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedUserPaymentLogStorageException =
                new FailedUserPaymentLogStorageException(sqlException);

            var expectedUserPaymentLogDependencyException =
                new UserPaymentLogDependencyException(failedUserPaymentLogStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);
            // when
            ValueTask<UserPaymentLog> deleteUserPaymentLogTask =
                this.userPaymentLogService.RemoveUserPaymentLogByIdAsync(someUserPaymentLogId);

            UserPaymentLogDependencyException actualUserPaymentLogDependencyException =
                await Assert.ThrowsAsync<UserPaymentLogDependencyException>(
                    deleteUserPaymentLogTask.AsTask);

            // then
            actualUserPaymentLogDependencyException.Should()
                .BeEquivalentTo(expectedUserPaymentLogDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserPaymentLogDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someUserPaymentLogId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedUserPaymentLogServiceException =
                new FailedUserPaymentLogServiceException(serviceException);

            var expectedUserPaymentLogServiceException =
                new UserPaymentLogServiceException(failedUserPaymentLogServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<UserPaymentLog> removeUserPaymentLogByIdTask =
                this.userPaymentLogService.RemoveUserPaymentLogByIdAsync(someUserPaymentLogId);

            UserPaymentLogServiceException actualUserPaymentLogServiceException =
                await Assert.ThrowsAsync<UserPaymentLogServiceException>(
                    removeUserPaymentLogByIdTask.AsTask);

            // then
            actualUserPaymentLogServiceException.Should()
                .BeEquivalentTo(expectedUserPaymentLogServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserPaymentLogServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
