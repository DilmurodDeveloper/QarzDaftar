using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public async Task ShouldModifyUserNoteAsync()
        {
            // given 
            UserNote randomUserNote = CreateRandomUserNote();
            UserNote inputUserNote = randomUserNote;
            UserNote persistedUserNote = inputUserNote.DeepClone();
            UserNote updatedUserNote = inputUserNote;
            UserNote expectedUserNote = updatedUserNote.DeepClone();
            Guid InputUserNoteId = inputUserNote.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectUserNoteByIdAsync(InputUserNoteId))
                    .ReturnsAsync(persistedUserNote);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateUserNoteAsync(inputUserNote))
                    .ReturnsAsync(updatedUserNote);

            // when
            UserNote actualUserNote =
                await this.userNoteService.ModifyUserNoteAsync(inputUserNote);

            // then
            actualUserNote.Should().BeEquivalentTo(expectedUserNote);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectUserNoteByIdAsync(InputUserNoteId), Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateUserNoteAsync(inputUserNote), Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
