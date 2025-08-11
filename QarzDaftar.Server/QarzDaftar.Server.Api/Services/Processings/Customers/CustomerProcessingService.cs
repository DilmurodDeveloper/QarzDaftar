using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Services.Foundations.Customers;

namespace QarzDaftar.Server.Api.Services.Processings.Customers
{
    public partial class CustomerProcessingService : ICustomerProcessingService
    {
        private readonly ICustomerService customerService;

        public CustomerProcessingService(ICustomerService customerService) =>
            this.customerService = customerService;

        public async ValueTask<Customer> AddCustomerAsync(Customer customer)
        {
            customer.Id = Guid.NewGuid();
            customer.CreatedDate = DateTimeOffset.UtcNow;
            customer.UpdatedDate = customer.CreatedDate;

            return await this.customerService.AddCustomerAsync(customer);
        }


        public IQueryable<Customer> RetrieveAllCustomers(Guid userId) =>
            this.customerService.RetrieveAllCustomers(userId);

        public async ValueTask<Customer> RetrieveCustomerByIdAsync(Guid customerId) =>
            await this.customerService.RetrieveCustomerByIdAsync(customerId);

        public async ValueTask<Customer> ModifyCustomerAsync(Customer customer) =>
            await this.customerService.ModifyCustomerAsync(customer);

        public async ValueTask<Customer> RemoveCustomerByIdAsync(Guid customerId, Guid userId)
        {
            Customer existingCustomer = await this.customerService
                .RetrieveCustomerByIdAsync(customerId);

            if (existingCustomer.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this customer.");
            }

            return await this.customerService.RemoveCustomerByIdAsync(customerId);
        }
    }
}
