using QarzDaftar.Server.Api.Models.Foundations.Users;

namespace QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories
{
    public class SubscriptionHistory
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset PurchasedAt { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
