using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions
{
    public class InvalidCustomerException : Xeption
    {
        public InvalidCustomerException()
            : base(message: "Customer is invalid.")
        { }
    }
}
