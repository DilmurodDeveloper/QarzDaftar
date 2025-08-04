using FluentAssertions;
using Force.DeepCloner;
using Moq;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;

namespace QarzDaftar.Server.Api.Tests.Unit.Services.Foundations.UserNotes
{
    public partial class UserNoteServiceTests
    {
        [Fact]
        public async Task ShouldAddUserNoteAsync()
        {
            // given
            UserNote randomUserNote = CreateRandomUserNote();
            UserNote inputUserNote = randomUserNote;
            UserNote persistedUserNote = inputUserNote;
            UserNote expectedUserNote = persistedUserNote.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertUserNoteAsync(inputUserNote))
                    .ReturnsAsync(persistedUserNote);

            // when
            UserNote actualUserNote =
                await this.userNoteService.AddUserNoteAsync(inputUserNote);

            // then
            actualUserNote.Should().BeEquivalentTo(expectedUserNote);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertUserNoteAsync(inputUserNote), Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
