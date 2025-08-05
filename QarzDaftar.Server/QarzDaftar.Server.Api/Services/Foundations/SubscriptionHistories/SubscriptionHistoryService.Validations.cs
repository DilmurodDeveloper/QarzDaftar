using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.SubscriptionHistories
{
    public partial class SubscriptionHistoryService
    {
        private void ValidateSubscriptionHistoryOnAdd(SubscriptionHistory subscriptionHistory)
        {
            ValidateSubscriptionHistoryNotNull(subscriptionHistory);

            Validate(
                (Rule: IsInvalid(subscriptionHistory.Id), Parameter: nameof(SubscriptionHistory.Id)),
                (Rule: IsInvalid(subscriptionHistory.Amount), Parameter: nameof(SubscriptionHistory.Amount)),
                (Rule: IsInvalid(subscriptionHistory.PurchasedAt), Parameter: nameof(SubscriptionHistory.PurchasedAt)),
                (Rule: IsInvalid(subscriptionHistory.ExpiresAt), Parameter: nameof(SubscriptionHistory.ExpiresAt)),
                (Rule: IsInvalid(subscriptionHistory.UserId), Parameter: nameof(SubscriptionHistory.UserId)));
        }

        private static void ValidateSubscriptionHistoryNotNull(
            SubscriptionHistory subscriptionHistory)
        {
            if (subscriptionHistory is null)
            {
                throw new NullSubscriptionHistoryException();
            }
        }

        private static dynamic IsInvalid(Guid Id) => new
        {
            Condition = Id == default,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(decimal amount) => new
        {
            Condition = amount <= 0,
            Message = "Amount is invalid"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private void ValidateSubscriptionHistoryId(Guid subscriptionHistoryId) =>
            Validate((Rule: IsInvalid(subscriptionHistoryId), Parameter: nameof(SubscriptionHistory.Id)));

        private static void ValidateStorageSubscriptionHistory(
            SubscriptionHistory maybeSubscriptionHistory, Guid subscriptionHistoryId)
        {
            if (maybeSubscriptionHistory is null)
            {
                throw new NotFoundSubscriptionHistoryException(subscriptionHistoryId);
            }
        }

        private void ValidateSubscriptionHistoryOnModify(SubscriptionHistory subscriptionHistory)
        {
            ValidateSubscriptionHistoryNotNull(subscriptionHistory);

            Validate(
                (Rule: IsInvalid(subscriptionHistory.Id), Parameter: nameof(SubscriptionHistory.Id)),
                (Rule: IsInvalid(subscriptionHistory.Amount), Parameter: nameof(SubscriptionHistory.Amount)),
                (Rule: IsInvalid(subscriptionHistory.PurchasedAt), Parameter: nameof(SubscriptionHistory.PurchasedAt)),
                (Rule: IsInvalid(subscriptionHistory.ExpiresAt), Parameter: nameof(SubscriptionHistory.ExpiresAt)),
                (Rule: IsInvalid(subscriptionHistory.UserId), Parameter: nameof(SubscriptionHistory.UserId)));
        }

        private void ValidateAgainstStorageSubscriptionHistoryOnModify(
            SubscriptionHistory subscriptionHistory, SubscriptionHistory storageSubscriptionHistory)
        {
            ValidateStorageSubscriptionHistory(storageSubscriptionHistory, subscriptionHistory.Id);

            Validate(
                (Rule: IsInvalid(subscriptionHistory.Id), Parameter: nameof(SubscriptionHistory.Id)),
                (Rule: IsInvalid(subscriptionHistory.Amount), Parameter: nameof(SubscriptionHistory.Amount)),
                (Rule: IsInvalid(subscriptionHistory.PurchasedAt), Parameter: nameof(SubscriptionHistory.PurchasedAt)),
                (Rule: IsInvalid(subscriptionHistory.ExpiresAt), Parameter: nameof(SubscriptionHistory.ExpiresAt)),
                (Rule: IsInvalid(subscriptionHistory.UserId), Parameter: nameof(SubscriptionHistory.UserId)));
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidSubscriptionHistoryException = new InvalidSubscriptionHistoryException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidSubscriptionHistoryException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidSubscriptionHistoryException.ThrowIfContainsErrors();
        }
    }
}
