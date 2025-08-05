using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            UserNote randomUserNote = CreateRandomUserNote();
            UserNote someUserNote = randomUserNote;
            Guid UserNoteId = someUserNote.Id;
            SqlException sqlException = GetSqlError();

            var failedUserNoteStorageException =
                new FailedUserNoteStorageException(sqlException);

            var expectedUserNoteDependencyException =
                new UserNoteDependencyException(failedUserNoteStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserNoteByIdAsync(UserNoteId)).Throws(sqlException);

            // when
            ValueTask<UserNote> modifyUserNoteTask =
                this.userNoteService.ModifyUserNoteAsync(someUserNote);

            UserNoteDependencyException actualUserNoteDependencyException =
                 await Assert.ThrowsAsync<UserNoteDependencyException>(
                    modifyUserNoteTask.AsTask);

            // then
            actualUserNoteDependencyException.Should()
                .BeEquivalentTo(expectedUserNoteDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserNoteDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(UserNoteId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserNoteAsync(someUserNote), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            UserNote randomUserNote = CreateRandomUserNote();
            UserNote someUserNote = randomUserNote;
            Guid UserNoteId = someUserNote.Id;
            var databaseUpdateException = new DbUpdateException();

            var failedUserNoteStorageException =
                new FailedUserNoteStorageException(databaseUpdateException);

            var expectedUserNoteDependencyException =
                new UserNoteDependencyException(failedUserNoteStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserNoteByIdAsync(UserNoteId))
                    .Throws(databaseUpdateException);

            // when
            ValueTask<UserNote> modifyUserNoteTask =
                this.userNoteService.ModifyUserNoteAsync(someUserNote);

            UserNoteDependencyException actualUserNoteDependencyException =
                 await Assert.ThrowsAsync<UserNoteDependencyException>(
                    modifyUserNoteTask.AsTask);

            // then
            actualUserNoteDependencyException.Should()
                .BeEquivalentTo(expectedUserNoteDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteDependencyException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(UserNoteId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserNoteAsync(someUserNote), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            UserNote randomUserNote = CreateRandomUserNote();
            UserNote someUserNote = randomUserNote;
            Guid UserNoteId = someUserNote.Id;
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedUserNoteException =
                new LockedUserNoteException(dbUpdateConcurrencyException);

            var expectedUserNoteDependencyValidationException =
                new UserNoteDependencyValidationException(lockedUserNoteException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserNoteByIdAsync(UserNoteId))
                    .Throws(dbUpdateConcurrencyException);

            // when
            ValueTask<UserNote> modifyUserNoteTask =
                this.userNoteService.ModifyUserNoteAsync(someUserNote);

            UserNoteDependencyValidationException actualUserNoteDependencyValidationException =
                 await Assert.ThrowsAsync<UserNoteDependencyValidationException>(
                    modifyUserNoteTask.AsTask);

            // then
            actualUserNoteDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserNoteDependencyValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteDependencyValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(UserNoteId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserNoteAsync(someUserNote), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            UserNote randomUserNote = CreateRandomUserNote();
            UserNote someUserNote = randomUserNote;
            Guid UserNoteId = someUserNote.Id;
            var serviceException = new Exception();

            var failedUserNoteServiceException =
                new FailedUserNoteServiceException(serviceException);

            var expectedUserNoteServiceException =
                new UserNoteServiceException(failedUserNoteServiceException);

            this.storageBrokerMock.Setup(broker =>
                    broker.SelectUserNoteByIdAsync(UserNoteId))
                .Throws(serviceException);

            // when
            ValueTask<UserNote> modifyUserNoteTask =
                this.userNoteService.ModifyUserNoteAsync(someUserNote);

            UserNoteServiceException actualUserNoteServiceException =
                await Assert.ThrowsAsync<UserNoteServiceException>(
                    modifyUserNoteTask.AsTask);

            // then
            actualUserNoteServiceException.Should()
                .BeEquivalentTo(expectedUserNoteServiceException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteServiceException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(UserNoteId), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserNoteAsync(someUserNote), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
