using System.Text.Json.Serialization;
using QarzDaftar.Server.Api.Models.Foundations.Customers;

namespace QarzDaftar.Server.Api.Models.Foundations.Payments
{
    public class Payment
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public PaymentMethod Method { get; set; }
        public DateTimeOffset PaymentDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public Guid CustomerId { get; set; }

        [JsonIgnore]
        public Customer? Customer { get; set; }
    }
}
