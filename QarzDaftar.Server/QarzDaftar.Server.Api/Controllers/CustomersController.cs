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

        [HttpGet("All")]
        public ActionResult<IQueryable<Customer>> GetAllCustomers()
        {
            try
            {
                IQueryable<Customer> allCustomers = this.customerService.RetrieveAllCustomers();

                return Ok(allCustomers);
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

        [HttpGet("ById")]
        public async ValueTask<ActionResult<Customer>> GetCustomerByIdAsync(Guid customerId)
        {
            try
            {
                return await this.customerService.RetrieveCustomerByIdAsync(customerId);
            }
            catch (CustomerDependencyException customerDependencyException)
            {
                return InternalServerError(customerDependencyException.InnerException);
            }
            catch (CustomerValidationException customerValidationException)
                when (customerValidationException.InnerException is InvalidCustomerException)
            {
                return BadRequest(customerValidationException.InnerException);
            }
            catch (CustomerValidationException customerValidationException)
                when (customerValidationException.InnerException is NotFoundCustomerException)
            {
                return NotFound(customerValidationException.InnerException);
            }
            catch (CustomerServiceException customerServiceException)
            {
                return InternalServerError(customerServiceException.InnerException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Customer>> PutCustomerAsync(Customer customer)
        {
            try
            {
                Customer modifyCustomer =
                    await this.customerService.ModifyCustomerAsync(customer);

                return Ok(modifyCustomer);
            }
            catch (CustomerValidationException customerValidationException)
                when (customerValidationException.InnerException is NotFoundCustomerException)
            {
                return NotFound(customerValidationException.InnerException);
            }
            catch (CustomerValidationException customerValidationException)
            {
                return BadRequest(customerValidationException.InnerException);
            }
            catch (CustomerDependencyValidationException customerDependencyValidationException)
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
