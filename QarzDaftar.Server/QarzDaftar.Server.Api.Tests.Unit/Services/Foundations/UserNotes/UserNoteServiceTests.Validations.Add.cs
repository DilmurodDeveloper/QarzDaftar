using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfInputIsNullAndLogItAsync()
        {
            // given
            UserNote nullUserNote = null;
            var nullUserNoteException = new NullUserNoteException();

            var expectedUserNoteValidationException =
                new UserNoteValidationException(nullUserNoteException);

            // when
            ValueTask<UserNote> addUserNoteTask =
                this.userNoteService.AddUserNoteAsync(nullUserNote);

            UserNoteValidationException actualUserNoteException =
                await Assert.ThrowsAsync<UserNoteValidationException>(
                    addUserNoteTask.AsTask);

            // then
            actualUserNoteException.Should()
                .BeEquivalentTo(expectedUserNoteValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserNoteAsync(It.IsAny<UserNote>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
