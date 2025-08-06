using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.SuperAdmins
{
    public partial class SuperAdminServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveSuperAdminByUsernameAsync()
        {
            // given
            string randomUsername = GetRandomUsername();
            string inputUsername = randomUsername;
            SuperAdmin randomSuperAdmin = CreateRandomSuperAdmin();
            randomSuperAdmin.Username = inputUsername;
            SuperAdmin persistedSuperAdmin = randomSuperAdmin;
            SuperAdmin expectedSuperAdmin = persistedSuperAdmin;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectSuperAdminByUsernameAsync(inputUsername))
                    .ReturnsAsync(persistedSuperAdmin);

            // when
            SuperAdmin actualSuperAdmin =
                await this.superAdminService
                    .RetrieveSuperAdminByUsernameAsync(inputUsername);

            // then 
            actualSuperAdmin.Should().BeEquivalentTo(expectedSuperAdmin);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectSuperAdminByUsernameAsync(inputUsername),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
