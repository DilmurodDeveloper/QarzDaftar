using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SuperAdmins
{
    public partial class SuperAdminServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByUsernameIfSqlErrorOccursAndLogItAsync()
        {
            // given
            string someUsername = GetRandomUsername();
            SqlException sqlException = GetSqlError();

            var failedStorageException = new FailedSuperAdminStorageException(sqlException);
            var expectedDependencyException = new SuperAdminDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSuperAdminByUsernameAsync(It.IsAny<string>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<SuperAdmin> retrieveSuperAdminTask =
                this.superAdminService.RetrieveSuperAdminByUsernameAsync(someUsername);

            SuperAdminDependencyException actualDependencyException =
                await Assert.ThrowsAsync<SuperAdminDependencyException>(
                    retrieveSuperAdminTask.AsTask);

            // then
            actualDependencyException.Should().BeEquivalentTo(expectedDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSuperAdminByUsernameAsync(It.IsAny<string>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(expectedDependencyException))),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByUsernameIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string someUsername = GetRandomUsername();
            var serviceException = new Exception();

            var failedServiceException = new FailedSuperAdminServiceException(serviceException);
            var expectedServiceException = new SuperAdminServiceException(failedServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSuperAdminByUsernameAsync(It.IsAny<string>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<SuperAdmin> retrieveSuperAdminTask =
                this.superAdminService.RetrieveSuperAdminByUsernameAsync(someUsername);

            SuperAdminServiceException actualServiceException =
                await Assert.ThrowsAsync<SuperAdminServiceException>(
                    retrieveSuperAdminTask.AsTask);

            // then
            actualServiceException.Should().BeEquivalentTo(expectedServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSuperAdminByUsernameAsync(It.IsAny<string>()),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedServiceException))),
                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
