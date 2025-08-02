using QarzDaftar.Server.Api.Models.Foundations.Customers;

namespace QarzDaftar.Server.Api.Services.Foundatios.Customers
{
    public interface ICustomerService
    {
        ValueTask<Customer> AddCustomerAsync(Customer customer);
    }
}
