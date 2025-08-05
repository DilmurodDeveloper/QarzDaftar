using QarzDaftar.Server.Api.Models.Enums;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.UserNotes
{
    public partial class UserNoteService
    {
        private void ValidateUserNoteOnAdd(UserNote userNote)
        {
            ValidateUserNoteNotNull(userNote);

            Validate(
                (Rule: IsInvalid(userNote.Id), Parameter: nameof(UserNote.Id)),
                (Rule: IsInvalid(userNote.Content), Parameter: nameof(UserNote.Content)),
                (Rule: IsInvalid(userNote.ReminderDate), Parameter: nameof(UserNote.ReminderDate)),
                (Rule: IsInvalid(userNote.CreatedAt), Parameter: nameof(UserNote.CreatedAt)),
                (Rule: IsInvalid(userNote.Status), Parameter: nameof(UserNote.Status)),
                (Rule: IsInvalid(userNote.UserId), Parameter: nameof(UserNote.UserId)));
        }

        private static void ValidateUserNoteNotNull(UserNote UserNote)
        {
            if (UserNote is null)
            {
                throw new NullUserNoteException();
            }
        }

        private static dynamic IsInvalid(Guid Id) => new
        {
            Condition = Id == default,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(ReminderStatus status) => new
        {
            Condition = Enum.IsDefined(typeof(ReminderStatus), status) is false,
            Message = "Value is invalid"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private void ValidateUserNoteId(Guid userNoteId) =>
            Validate((Rule: IsInvalid(userNoteId), Parameter: nameof(UserNote.Id)));

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserNoteException = new InvalidUserNoteException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserNoteException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidUserNoteException.ThrowIfContainsErrors();
        }
    }
}
