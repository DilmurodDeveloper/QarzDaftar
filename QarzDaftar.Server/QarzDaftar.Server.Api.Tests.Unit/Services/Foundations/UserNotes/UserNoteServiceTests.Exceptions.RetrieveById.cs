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
        public async Task ShouldThrowSqlExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = GetSqlError();

            var failedUserNoteStorageException =
                new FailedUserNoteStorageException(sqlException);

            var expectedUserNoteDependencyException =
                new UserNoteDependencyException(failedUserNoteStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserNoteByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<UserNote> retrieveUserNoteByIdTask =
                this.userNoteService.RetrieveUserNoteByIdAsync(someId);

            UserNoteDependencyException actualUserNoteDependencyException =
                await Assert.ThrowsAsync<UserNoteDependencyException>(
                    retrieveUserNoteByIdTask.AsTask);

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
