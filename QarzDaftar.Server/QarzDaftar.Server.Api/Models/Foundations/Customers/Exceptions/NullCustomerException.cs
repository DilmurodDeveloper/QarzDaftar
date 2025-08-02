using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions
{
    public class NullCustomerException : Xeption
    {
        public NullCustomerException()
            : base(message: "Customer is null.")
        { }
    }
}
