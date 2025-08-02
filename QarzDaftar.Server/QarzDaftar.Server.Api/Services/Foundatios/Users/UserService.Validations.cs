using QarzDaftar.Server.Api.Models.Foundations.Users;
using QarzDaftar.Server.Api.Models.Foundations.Users.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundatios.Users
{
    public partial class UserService
    {
        private void ValidateUserOnAdd(User user)
        {
            ValidateUserNotNull(user);

            Validate(
                (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
                (Rule: IsInvalid(user.FullName), Parameter: nameof(User.FullName)),
                (Rule: IsInvalid(user.Username), Parameter: nameof(User.Username)),
                (Rule: IsInvalid(user.Email), Parameter: nameof(User.Email)),
                (Rule: IsInvalid(user.PhoneNumber), Parameter: nameof(User.PhoneNumber)),
                (Rule: IsInvalid(user.PasswordHash), Parameter: nameof(User.PasswordHash)),
                (Rule: IsInvalid(user.ShopName), Parameter: nameof(User.ShopName)),
                (Rule: IsInvalid(user.Address), Parameter: nameof(User.Address)),
                (Rule: IsInvalid(user.RegisteredAt), Parameter: nameof(User.RegisteredAt)),
                (Rule: IsInvalid(user.SubscriptionExpiresAt), Parameter: nameof(User.SubscriptionExpiresAt)),
                (Rule: IsInvalid(user.CreatedDate), Parameter: nameof(User.CreatedDate)),
                (Rule: IsInvalid(user.UpdatedDate), Parameter: nameof(User.UpdatedDate)));
        }

        private static void ValidateUserNotNull(User user)
        {
            if (user is null)
            {
                throw new NullUserException();
            }
        }

        private static void ValidateUserId(Guid userId) =>
            Validate((Rule: IsInvalid(userId), Parameter: nameof(User.Id)));

        private static void ValidateStorageUser(User maybeUser, Guid userId)
        {
            if (maybeUser is null)
            {
                throw new NotFoundUserException(userId);
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

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static void ValidateUserOnModify(User user)
        {
            ValidateUserNotNull(user);

            Validate(
                (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
                (Rule: IsInvalid(user.FullName), Parameter: nameof(User.FullName)),
                (Rule: IsInvalid(user.Username), Parameter: nameof(User.Username)),
                (Rule: IsInvalid(user.Email), Parameter: nameof(User.Email)),
                (Rule: IsInvalid(user.PhoneNumber), Parameter: nameof(User.PhoneNumber)),
                (Rule: IsInvalid(user.PasswordHash), Parameter: nameof(User.PasswordHash)),
                (Rule: IsInvalid(user.ShopName), Parameter: nameof(User.ShopName)),
                (Rule: IsInvalid(user.Address), Parameter: nameof(User.Address)),
                (Rule: IsInvalid(user.RegisteredAt), Parameter: nameof(User.RegisteredAt)),
                (Rule: IsInvalid(user.SubscriptionExpiresAt), Parameter: nameof(User.SubscriptionExpiresAt)),
                (Rule: IsInvalid(user.CreatedDate), Parameter: nameof(User.CreatedDate)),
                (Rule: IsInvalid(user.UpdatedDate), Parameter: nameof(User.UpdatedDate)));
        }

        private static void ValidateAgainstStorageUserOnModify(User user, User storageUser)
        {
            ValidateStorageUser(storageUser, user.Id);

            Validate(
                (Rule: IsInvalid(user.Id), Parameter: nameof(User.Id)),
                (Rule: IsInvalid(user.FullName), Parameter: nameof(User.FullName)),
                (Rule: IsInvalid(user.Username), Parameter: nameof(User.Username)),
                (Rule: IsInvalid(user.Email), Parameter: nameof(User.Email)),
                (Rule: IsInvalid(user.PhoneNumber), Parameter: nameof(User.PhoneNumber)),
                (Rule: IsInvalid(user.PasswordHash), Parameter: nameof(User.PasswordHash)),
                (Rule: IsInvalid(user.ShopName), Parameter: nameof(User.ShopName)),
                (Rule: IsInvalid(user.Address), Parameter: nameof(User.Address)),
                (Rule: IsInvalid(user.RegisteredAt), Parameter: nameof(User.RegisteredAt)),
                (Rule: IsInvalid(user.SubscriptionExpiresAt), Parameter: nameof(User.SubscriptionExpiresAt)),
                (Rule: IsInvalid(user.CreatedDate), Parameter: nameof(User.CreatedDate)),
                (Rule: IsInvalid(user.UpdatedDate), Parameter: nameof(User.UpdatedDate)));
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidUserException = new InvalidUserException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidUserException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidUserException.ThrowIfContainsErrors();
        }
    }
}
