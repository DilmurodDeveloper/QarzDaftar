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
    }
}
