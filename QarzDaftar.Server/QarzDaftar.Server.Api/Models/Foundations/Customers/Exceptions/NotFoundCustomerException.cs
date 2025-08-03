using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions
{
    public class NotFoundCustomerException : Xeption
    {
        public NotFoundCustomerException(Guid customerId)
            : base(message: $"Customer is not found with id: {customerId}")
        { }
    }
}
