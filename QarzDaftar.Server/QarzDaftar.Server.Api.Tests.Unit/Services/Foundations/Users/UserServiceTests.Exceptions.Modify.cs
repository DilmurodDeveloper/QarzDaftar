using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.Users;
using QarzDaftar.Server.Api.Models.Foundations.Users.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.Users
{
    public partial class UserServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            User randomUser = CreateRandomUser();
            User someUser = randomUser;
            Guid UserId = someUser.Id;
            SqlException sqlException = GetSqlError();

            var failedUserStorageException =
                new FailedUserStorageException(sqlException);

            var expectedUserDependencyException =
                new UserDependencyException(failedUserStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(UserId)).Throws(sqlException);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(someUser);

            UserDependencyException actualUserDependencyException =
                 await Assert.ThrowsAsync<UserDependencyException>(
                    modifyUserTask.AsTask);

            // then
            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(UserId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(someUser), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            User randomUser = CreateRandomUser();
            User someUser = randomUser;
            Guid UserId = someUser.Id;
            var databaseUpdateException = new DbUpdateException();

            var failedUserStorageException =
                new FailedUserStorageException(databaseUpdateException);

            var expectedUserDependencyException =
                new UserDependencyException(failedUserStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(UserId)).Throws(databaseUpdateException);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(someUser);

            UserDependencyException actualUserDependencyException =
                 await Assert.ThrowsAsync<UserDependencyException>(
                    modifyUserTask.AsTask);

            // then
            actualUserDependencyException.Should()
                .BeEquivalentTo(expectedUserDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(UserId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(someUser), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            User randomUser = CreateRandomUser();
            User someUser = randomUser;
            Guid UserId = someUser.Id;
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedUserException =
                new LockedUserException(dbUpdateConcurrencyException);

            var expectedUserDependencyValidationException =
                new UserDependencyValidationException(lockedUserException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserByIdAsync(UserId))
                    .Throws(dbUpdateConcurrencyException);

            // when
            ValueTask<User> modifyUserTask =
                this.userService.ModifyUserAsync(someUser);

            UserDependencyValidationException actualUserDependencyValidationException =
                 await Assert.ThrowsAsync<UserDependencyValidationException>(
                    modifyUserTask.AsTask);

            // then
            actualUserDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserByIdAsync(UserId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserAsync(someUser), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
