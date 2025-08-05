using FluentAssertions;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionIfUserNoteIsNullAndLogItAsync()
        {
            // given
            UserNote nullUserNote = null;
            var nullUserNoteException = new NullUserNoteException();

            var expectedUserNoteValidationException =
                new UserNoteValidationException(nullUserNoteException);

            // when
            ValueTask<UserNote> modifyUserNoteTask =
                this.userNoteService.ModifyUserNoteAsync(nullUserNote);

            UserNoteValidationException actualUserNoteValidationException =
                await Assert.ThrowsAsync<UserNoteValidationException>(
                    modifyUserNoteTask.AsTask);

            // then
            actualUserNoteValidationException.Should()
                .BeEquivalentTo(expectedUserNoteValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserNoteAsync(It.IsAny<UserNote>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
