using QarzDaftar.Server.Api.Models.Foundations.Users;

namespace QarzDaftar.Server.Api.Models.Foundations.UserPaymentLogs
{
    public class UserPaymentLog
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Purpose { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset PaidAt { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
