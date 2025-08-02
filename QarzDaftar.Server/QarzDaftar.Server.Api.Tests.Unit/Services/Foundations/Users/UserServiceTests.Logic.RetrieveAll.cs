using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Users;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllUsers()
        {
            // given
            IQueryable<User> randomUsers = CreateRandomUsers();
            IQueryable<User> persistedUsers = randomUsers;
            IQueryable<User> expectedUsers = persistedUsers.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUsers()).Returns(persistedUsers);

            // when
            IQueryable<User> actualUsers =
                this.userService.RetrieveAllUsers();

            // then
            actualUsers.Should().BeEquivalentTo(expectedUsers);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUsers(), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
