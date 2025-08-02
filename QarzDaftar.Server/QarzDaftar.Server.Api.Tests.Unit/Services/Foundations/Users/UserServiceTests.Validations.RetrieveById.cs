using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Users;
using QarzDaftar.Server.Api.Models.Foundations.Users.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidUserId = Guid.Empty;
            var invalidUserException = new InvalidUserException();

            invalidUserException.AddData(
                key: nameof(User.Id),
                values: "Id is required");

            var expectedUserValidationException =
                new UserValidationException(invalidUserException);

            // when
            ValueTask<User> retrieveUserById =
                this.userService.RetrieveUserByIdAsync(invalidUserId);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    retrieveUserById.AsTask);

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfUserNotFoundAndLogItAsync()
        {
            // given
            Guid someUserId = Guid.NewGuid();
            User noUser = null;

            var notFoundUserException =
                new NotFoundUserException(someUserId);

            var expetedUserValidationException =
                new UserValidationException(notFoundUserException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noUser);

            // when
            ValueTask<User> retriveByIdUserTask =
                this.userService.RetrieveUserByIdAsync(someUserId);

            var actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(
                    retriveByIdUserTask.AsTask);

            // then
            actualUserValidationException.Should().BeEquivalentTo(expetedUserValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(someUserId), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
            broker.LogError(It.Is(SameExceptionAs(
                expetedUserValidationException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
