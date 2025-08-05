using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someUserNoteId = Guid.NewGuid();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedUserNoteException =
                new LockedUserNoteException(dbUpdateConcurrencyException);

            var expectedUserNoteDependencyValidationException =
                new UserNoteDependencyValidationException(lockedUserNoteException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserNoteByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<UserNote> removeUserNoteById =
                this.userNoteService.RemoveUserNoteByIdAsync(someUserNoteId);

            var actualUserNoteDependencyValidationException =
                await Assert.ThrowsAsync<UserNoteDependencyValidationException>(
                    removeUserNoteById.AsTask);

            // then
            actualUserNoteDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserNoteDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(It.IsAny<Guid>()), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteUserNoteAsync(It.IsAny<UserNote>()), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someUserNoteId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedUserNoteStorageException =
                new FailedUserNoteStorageException(sqlException);

            var expectedUserNoteDependencyException =
                new UserNoteDependencyException(failedUserNoteStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserNoteByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);
            // when
            ValueTask<UserNote> deleteUserNoteTask =
                this.userNoteService.RemoveUserNoteByIdAsync(someUserNoteId);

            UserNoteDependencyException actualUserNoteDependencyException =
                await Assert.ThrowsAsync<UserNoteDependencyException>(
                    deleteUserNoteTask.AsTask);

            // then
            actualUserNoteDependencyException.Should()
                .BeEquivalentTo(expectedUserNoteDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserNoteDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
