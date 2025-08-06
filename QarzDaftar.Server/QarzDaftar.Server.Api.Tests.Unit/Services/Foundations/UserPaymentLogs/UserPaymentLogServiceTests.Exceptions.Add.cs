using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursLogItAsync()
        {
            // given
            UserPaymentLog someUserPaymentLog = CreateRandomUserPaymentLog();
            SqlException sqlException = GetSqlError();

            var failedUserPaymentLogStorageException =
                new FailedUserPaymentLogStorageException(sqlException);

            var expectedUserPaymentLogDependencyException =
                new UserPaymentLogDependencyException(failedUserPaymentLogStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertUserPaymentLogAsync(someUserPaymentLog))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<UserPaymentLog> addUserPaymentLogTask =
                this.userPaymentLogService.AddUserPaymentLogAsync(someUserPaymentLog);

            // then
            await Assert.ThrowsAsync<UserPaymentLogDependencyException>(() =>
                addUserPaymentLogTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserPaymentLogAsync(someUserPaymentLog), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserPaymentLogDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccursAndLogItAsync()
        {
            // given
            string someMessage = GetRandomString();
            UserPaymentLog someUserPaymentLog = CreateRandomUserPaymentLog();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsUserPaymentLogException =
                new AlreadyExistsUserPaymentLogException(duplicateKeyException);

            var expectedUserPaymentLogDependencyValidationException =
                new UserPaymentLogDependencyValidationException(alreadyExistsUserPaymentLogException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertUserPaymentLogAsync(someUserPaymentLog)).ThrowsAsync(duplicateKeyException);
            // when
            ValueTask<UserPaymentLog> addUserPaymentLogTask =
                this.userPaymentLogService.AddUserPaymentLogAsync(someUserPaymentLog);

            UserPaymentLogDependencyValidationException actualUserPaymentLogDependencyValidationException =
                await Assert.ThrowsAsync<UserPaymentLogDependencyValidationException>(
                    addUserPaymentLogTask.AsTask);

            // then
            actualUserPaymentLogDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserPaymentLogDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserPaymentLogAsync(someUserPaymentLog), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserPaymentLogDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
