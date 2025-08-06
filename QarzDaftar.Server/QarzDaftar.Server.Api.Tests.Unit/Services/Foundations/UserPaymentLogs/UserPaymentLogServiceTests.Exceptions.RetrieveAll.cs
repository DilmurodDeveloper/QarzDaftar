using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserPaymentLogs
{
    public partial class UserPaymentLogServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given 
            SqlException sqlException = GetSqlError();

            var failedUserPaymentLogStorageException =
                new FailedUserPaymentLogStorageException(sqlException);

            var expectedUserPaymentLogDependencyException =
                new UserPaymentLogDependencyException(failedUserPaymentLogStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUserPaymentLogs()).Throws(sqlException);

            // when
            Action retrieveAllUserPaymentLogsAction = () =>
                this.userPaymentLogService.RetrieveAllUserPaymentLogs();

            var actualUserPaymentLogDependencyException =
                Assert.Throws<UserPaymentLogDependencyException>(
                    retrieveAllUserPaymentLogsAction);

            // then
            actualUserPaymentLogDependencyException.Should()
                .BeEquivalentTo(expectedUserPaymentLogDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUserPaymentLogs(),
                    Times.Once());

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
