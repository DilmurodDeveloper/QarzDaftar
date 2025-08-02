using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.Customers
{
    public partial class CustomerService
    {
        private static void ValidateCustomerNotNull(Customer customer)
        {
            if (customer is null)
            {
                throw new NullCustomerException();
            }
        }
    }
}
