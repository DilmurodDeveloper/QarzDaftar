using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursLogItAsync()
        {
            // given
            UserNote someUserNote = CreateRandomUserNote();
            SqlException sqlException = GetSqlError();

            var failedUserNoteStorageException =
                new FailedUserNoteStorageException(sqlException);

            var expectedUserNoteDependencyException =
                new UserNoteDependencyException(failedUserNoteStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertUserNoteAsync(someUserNote))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<UserNote> addUserNoteTask =
                this.userNoteService.AddUserNoteAsync(someUserNote);

            // then
            await Assert.ThrowsAsync<UserNoteDependencyException>(() =>
                addUserNoteTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserNoteAsync(someUserNote), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedUserNoteDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorOccursAndLogItAsync()
        {
            // given
            string someMessage = GetRandomString();
            UserNote someUserNote = CreateRandomUserNote();
            var duplicateKeyException = new DuplicateKeyException(someMessage);

            var alreadyExistsUserNoteException =
                new AlreadyExistsUserNoteException(duplicateKeyException);

            var expectedUserNoteDependencyValidationException =
                new UserNoteDependencyValidationException(alreadyExistsUserNoteException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertUserNoteAsync(someUserNote)).ThrowsAsync(duplicateKeyException);
            // when
            ValueTask<UserNote> addUserNoteTask =
                this.userNoteService.AddUserNoteAsync(someUserNote);

            UserNoteDependencyValidationException actualUserNoteDependencyValidationException =
                await Assert.ThrowsAsync<UserNoteDependencyValidationException>(
                    addUserNoteTask.AsTask);

            // then
            actualUserNoteDependencyValidationException.Should()
                .BeEquivalentTo(expectedUserNoteDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserNoteAsync(someUserNote), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            UserNote someUserNote = CreateRandomUserNote();
            var serviceException = new Exception();

            var failedUserNoteServiceException =
                new FailedUserNoteServiceException(serviceException);

            var expectedUserNoteServiceException =
                new UserNoteServiceException(failedUserNoteServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertUserNoteAsync(someUserNote))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<UserNote> addUserNoteTask =
                this.userNoteService.AddUserNoteAsync(someUserNote);

            UserNoteServiceException actualUserNoteService =
                await Assert.ThrowsAsync<UserNoteServiceException>(
                    addUserNoteTask.AsTask);

            // then
            actualUserNoteService.Should()
                .BeEquivalentTo(expectedUserNoteServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserNoteAsync(someUserNote), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteServiceException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
