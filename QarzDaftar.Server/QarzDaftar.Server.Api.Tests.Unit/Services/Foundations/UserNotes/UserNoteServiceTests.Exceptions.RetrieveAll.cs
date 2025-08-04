using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given 
            SqlException sqlException = GetSqlError();

            var failedUserNoteStorageException =
                new FailedUserNoteStorageException(sqlException);

            var expectedUserNoteDependencyException =
                new UserNoteDependencyException(failedUserNoteStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllUserNotes()).Throws(sqlException);

            // when
            Action retrieveAllUserNotesAction = () =>
                this.userNoteService.RetrieveAllUserNotes();

            UserNoteDependencyException actualUserNoteDependencyException =
                Assert.Throws<UserNoteDependencyException>(retrieveAllUserNotesAction);

            // then
            actualUserNoteDependencyException.Should()
                .BeEquivalentTo(expectedUserNoteDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllUserNotes(), Times.Once());

            this.loggingBrokerMock.Verify(broker =>
            broker.LogCritical(It.Is(SameExceptionAs(
                expectedUserNoteDependencyException))), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
