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
        public async Task ShouldThrowSqlExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedUserPaymentLogStorageException =
                new FailedUserPaymentLogStorageException(sqlException);

            var expectedUserPaymentLogDependencyException =
                new UserPaymentLogDependencyException(
                    failedUserPaymentLogStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<UserPaymentLog> retrieveUserPaymentLogByIdTask =
                this.userPaymentLogService.RetrieveUserPaymentLogByIdAsync(someId);

            UserPaymentLogDependencyException actualUserPaymentLogDependencyException =
                await Assert.ThrowsAsync<UserPaymentLogDependencyException>(
                    retrieveUserPaymentLogByIdTask.AsTask);

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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedUserPaymentLogServiceException =
                new FailedUserPaymentLogServiceException(serviceException);

            var expectedUserPaymentLogServiceException =
                new UserPaymentLogServiceException(failedUserPaymentLogServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserPaymentLogByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<UserPaymentLog> retrieveUserPaymentLogByIdTask =
                this.userPaymentLogService.RetrieveUserPaymentLogByIdAsync(someId);

            UserPaymentLogServiceException actualUserPaymentLogServiceException =
                await Assert.ThrowsAsync<UserPaymentLogServiceException>(
                    retrieveUserPaymentLogByIdTask.AsTask);

            // then
            actualUserPaymentLogServiceException.Should()
                .BeEquivalentTo(expectedUserPaymentLogServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserPaymentLogByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

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
