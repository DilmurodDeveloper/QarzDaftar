using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SuperAdmins
{
    public partial class SuperAdminServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByUsernameIfUsernameIsInvalidAndLogItAsync()
        {
            // given
            string invalidUsername = null;
            var invalidSuperAdminException = new InvalidSuperAdminException();

            invalidSuperAdminException.AddData(
                key: nameof(SuperAdmin.Username),
                values: "Username is required");

            var expectedValidationException =
                new SuperAdminValidationException(invalidSuperAdminException);

            // when
            ValueTask<SuperAdmin> retrieveSuperAdminTask =
                this.superAdminService.RetrieveSuperAdminByUsernameAsync(invalidUsername);

            SuperAdminValidationException actualValidationException =
                await Assert.ThrowsAsync<SuperAdminValidationException>(
                    retrieveSuperAdminTask.AsTask);

            // then
            actualValidationException.Should().BeEquivalentTo(expectedValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSuperAdminByUsernameAsync(It.IsAny<string>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByUsernameIfSuperAdminNotFoundAndLogItAsync()
        {
            // given
            string someUsername = GetRandomString();
            SuperAdmin noSuperAdmin = null;

            var notFoundSuperAdminException =
                new NotFoundSuperAdminException(someUsername);

            var expectedValidationException =
                new SuperAdminValidationException(notFoundSuperAdminException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSuperAdminByUsernameAsync(It.IsAny<string>()))
                    .ReturnsAsync(noSuperAdmin);

            // when
            ValueTask<SuperAdmin> retrieveSuperAdminTask =
                this.superAdminService.RetrieveSuperAdminByUsernameAsync(someUsername);

            SuperAdminValidationException actualValidationException =
                await Assert.ThrowsAsync<SuperAdminValidationException>(
                    retrieveSuperAdminTask.AsTask);

            // then
            actualValidationException.Should().BeEquivalentTo(expectedValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSuperAdminByUsernameAsync(someUsername), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
