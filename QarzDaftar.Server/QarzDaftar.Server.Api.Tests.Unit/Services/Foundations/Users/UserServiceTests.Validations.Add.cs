using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Users;
using QarzDaftar.Server.Api.Models.Foundations.Users.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            User nullUser = null;
            var nullUserException = new NullUserException();

            var expectedUserValidationException =
                new UserValidationException(nullUserException);

            // when
            ValueTask<User> addUserTask =
                this.userService.AddUserAsync(nullUser);

            UserValidationException actualUserException =
                await Assert.ThrowsAsync<UserValidationException>(addUserTask.AsTask);

            // then
            actualUserException.Should().BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfUserIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidUser = new User
            {
                FullName = invalidText,
            };

            var invalidUserException = new InvalidUserException();

            invalidUserException.AddData(
                key: nameof(User.Id),
                values: "Id is required");

            invalidUserException.AddData(
                key: nameof(User.FullName),
                values: "Text is required");

            invalidUserException.AddData(
                key: nameof(User.Username),
                values: "Text is required");

            invalidUserException.AddData(
                key: nameof(User.Email),
                values: "Text is required");

            invalidUserException.AddData(
                key: nameof(User.PhoneNumber),
                values: "Text is required");

            invalidUserException.AddData(
                key: nameof(User.PasswordHash),
                values: "Text is required");

            invalidUserException.AddData(
                key: nameof(User.ShopName),
                values: "Text is required");

            invalidUserException.AddData(
                key: nameof(User.RegisteredAt),
                values: "Date is required");

            invalidUserException.AddData(
                key: nameof(User.SubscriptionExpiresAt),
                values: "Date is required");

            invalidUserException.AddData(
                key: nameof(User.CreatedDate),
                values: "Date is required");

            invalidUserException.AddData(
                key: nameof(User.UpdatedDate),
                values: "Date is required");

            var expectedUserValidationException =
                new UserValidationException(invalidUserException);

            // when
            ValueTask<User> addUserTask =
                this.userService.AddUserAsync(invalidUser);

            UserValidationException actualUserValidationException =
                await Assert.ThrowsAsync<UserValidationException>(addUserTask.AsTask);

            // then
            actualUserValidationException.Should()
                .BeEquivalentTo(expectedUserValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserAsync(It.IsAny<User>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
