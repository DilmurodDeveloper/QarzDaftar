using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions
{
    public class LockedCustomerException : Xeption
    {
        public LockedCustomerException(Exception innerException)
            : base(message: "Customer is locked, try again later.",
                  innerException)
        { }
    }
}
