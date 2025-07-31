using QarzDaftar.Server.Api.Models.Enums;
using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Users;

namespace QarzDaftar.Server.Api.Models.Foundations.Debts
{
    public class Debt
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DebtStatus Status { get; set; }
        public DateTimeOffset DueDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
