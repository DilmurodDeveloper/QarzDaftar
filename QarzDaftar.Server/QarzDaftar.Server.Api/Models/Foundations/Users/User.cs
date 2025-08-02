using System.Text.Json.Serialization;
using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Debts;
using QarzDaftar.Server.Api.Models.Foundations.SubscriptionHistories;
using QarzDaftar.Server.Api.Models.Foundations.UserNotes;
using QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs;

namespace QarzDaftar.Server.Api.Models.Foundations.Users
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string PasswordHash { get; set; }
        public string ShopName { get; set; }
        public string Address { get; set; }
        public DateTimeOffset RegisteredAt { get; set; }
        public DateTimeOffset SubscriptionExpiresAt { get; set; }
        public bool IsActivatedByAdmin { get; set; }
        public bool IsBlocked { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        [JsonIgnore]
        public ICollection<Customer>? Customers { get; set; }

        [JsonIgnore]
        public ICollection<Debt>? Debts { get; set; }

        [JsonIgnore]
        public ICollection<UserNote>? UserNotes { get; set; }

        [JsonIgnore]
        public ICollection<SubscriptionHistory>? SubscriptionHistories { get; set; }

        [JsonIgnore]
        public ICollection<UserPaymentLog>? PaymentLogs { get; set; }
    }
}
