using Microsoft.AspNetCore.Mvc;
using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions;
using QarzDaftar.Server.Api.Services.Foundations.Customers;
using RESTFulSense.Controllers;

namespace QarzDaftar.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : RESTFulController
    {
        private readonly ICustomerService customerService;

        public CustomersController(ICustomerService customerService) =>
            this.customerService = customerService;

        [HttpPost]
        public async ValueTask<ActionResult<Customer>> PostCustomerAsync(Customer customer)
        {
            try
            {
                Customer addCustomer = await this.customerService.AddCustomerAsync(customer);

                return Created(addCustomer);
            }
            catch (CustomerValidationException customerValidationException)
            {
                return BadRequest(customerValidationException.InnerException);
            }
            catch (CustomerDependencyValidationException customerDependencyValidationException)
                when (customerDependencyValidationException.InnerException is AlreadyExistsCustomerException)
            {
                return Conflict(customerDependencyValidationException.InnerException);
            }
            catch (CustomerDependencyException customerDependencyException)
            {
                return InternalServerError(customerDependencyException.InnerException);
            }
            catch (CustomerServiceException customerServiceException)
            {
                return InternalServerError(customerServiceException.InnerException);
            }
        }
    }
}
