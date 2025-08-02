using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions
{
    public class AlreadyExistsCustomerException : Xeption
    {
        public AlreadyExistsCustomerException(Exception innerException)
            : base(message: "Customer already exists.", innerException)
        { }
    }
}
