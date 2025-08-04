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
    }
}
