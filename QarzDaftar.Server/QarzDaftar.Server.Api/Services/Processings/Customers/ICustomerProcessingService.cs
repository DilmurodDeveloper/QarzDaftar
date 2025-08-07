using QarzDaftar.Server.Api.Models.Foundations.Customers;

namespace QarzDaftar.Server.Api.Services.Processings.Customers
{
    public interface ICustomerProcessingService
    {
        ValueTask<Customer> AddCustomerAsync(Customer customer);
        IQueryable<Customer> RetrieveAllCustomers(Guid userId);
        ValueTask<Customer> RetrieveCustomerByIdAsync(Guid customerId);
        ValueTask<Customer> ModifyCustomerAsync(Customer customer);
        ValueTask<Customer> RemoveCustomerByIdAsync(Guid customerId, Guid userId);
    }
}
