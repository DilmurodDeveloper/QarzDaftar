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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserNoteIsInvalidAndLogItAsync(
            string invalidString)
        {
            //given
            UserNote invalidUserNote = new UserNote
            {
                Content = invalidString
            };

            var invalidUserNoteException = new InvalidUserNoteException();

            invalidUserNoteException.AddData(
                key: nameof(UserNote.Id),
                values: "Id is required");

            invalidUserNoteException.AddData(
                key: nameof(UserNote.Content),
                values: "Text is required");

            invalidUserNoteException.AddData(
                key: nameof(UserNote.ReminderDate),
                values: "Date is required");

            invalidUserNoteException.AddData(
                key: nameof(UserNote.CreatedAt),
                values: "Date is required");

            invalidUserNoteException.AddData(
                key: nameof(UserNote.UserId),
                values: "Id is required");

            var expectedUserNoteValidationException =
                new UserNoteValidationException(invalidUserNoteException);

            // when
            ValueTask<UserNote> modifyUserNoteTask =
                this.userNoteService.ModifyUserNoteAsync(invalidUserNote);

            UserNoteValidationException actualUserNoteValidationException =
                await Assert.ThrowsAsync<UserNoteValidationException>(
                    modifyUserNoteTask.AsTask);

            // then
            actualUserNoteValidationException.Should()
                .BeEquivalentTo(expectedUserNoteValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserNoteAsync(It.IsAny<UserNote>()), Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUserNoteDoesNotExistAndLogItAsync()
        {
            // given
            UserNote randomUserNote = CreateRandomUserNote();
            UserNote nonExistUserNote = randomUserNote;
            UserNote nullUserNote = null;

            var notFoundUserNoteException =
                new NotFoundUserNoteException(nonExistUserNote.Id);

            var expectedUserNoteValidationException =
                new UserNoteValidationException(notFoundUserNoteException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserNoteByIdAsync(nonExistUserNote.Id))
                    .ReturnsAsync(nullUserNote);

            // when
            ValueTask<UserNote> modifyUserNoteTask =
                this.userNoteService.ModifyUserNoteAsync(nonExistUserNote);

            UserNoteValidationException actualUserNoteValidationException =
                await Assert.ThrowsAsync<UserNoteValidationException>
                    (modifyUserNoteTask.AsTask);

            // then
            actualUserNoteValidationException.Should()
                .BeEquivalentTo(expectedUserNoteValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(nonExistUserNote.Id), Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedUserNoteValidationException))), Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserNoteAsync(nonExistUserNote), Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
