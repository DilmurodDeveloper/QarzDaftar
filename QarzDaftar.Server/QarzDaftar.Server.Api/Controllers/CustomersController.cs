using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions;
using QarzDaftar.Server.Api.Services.Processings.Customers;
using RESTFulSense.Controllers;

namespace QarzDaftar.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CustomersController : RESTFulController
    {
        private readonly ICustomerProcessingService customerProcessingService;

        public CustomersController(ICustomerProcessingService customerProcessingService) =>
            this.customerProcessingService = customerProcessingService;

        [HttpPost]
        public async ValueTask<ActionResult<Customer>> PostCustomerAsync(Customer customer)
        {
            try
            {
                customer.UserId = GetCurrentUserId();

                Customer addCustomer =
                    await this.customerProcessingService.AddCustomerAsync(customer);

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
                Guid userId = GetCurrentUserId();

                IQueryable<Customer> allCustomers =
                    this.customerProcessingService.RetrieveAllCustomers(userId);

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
                Guid userId = GetCurrentUserId();

                Customer customer =
                    await this.customerProcessingService.RetrieveCustomerByIdAsync(customerId);

                if (customer == null || customer.UserId != userId)
                    return NotFound();

                return Ok(customer);
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
                Guid userId = GetCurrentUserId();

                if (customer.UserId != userId)
                    return Unauthorized("You cannot modify another user's customer.");

                Customer modifyCustomer =
                    await this.customerProcessingService.ModifyCustomerAsync(customer);

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

        [HttpDelete]
        public async ValueTask<ActionResult<Customer>> DeleteCustomerAsync(Guid customerId)
        {
            try
            {
                Guid userId = GetCurrentUserId();

                Customer deleteCustomer =
                    await this.customerProcessingService.RemoveCustomerByIdAsync(customerId, userId);

                return Ok(deleteCustomer);
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
                when (customerDependencyValidationException.InnerException is LockedCustomerException)
            {
                return Locked(customerDependencyValidationException.InnerException);
            }
            catch (CustomerDependencyValidationException customerDependencyValidationException)
            {
                return BadRequest(customerDependencyValidationException.InnerException);
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

        private Guid GetCurrentUserId()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }

            return userId;
        }
    }
}
