using QarzDaftar.Server.Api.Models.Foundations.Customers;
using QarzDaftar.Server.Api.Models.Foundations.Customers.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.Customers
{
    public partial class CustomerService
    {
        private void ValidateCustomerOnAdd(Customer customer)
        {
            ValidateCustomerNotNull(customer);

            Validate(
                (Rule: IsInvalid(customer.Id), Parameter: nameof(Customer.Id)),
                (Rule: IsInvalid(customer.FullName), Parameter: nameof(Customer.FullName)),
                (Rule: IsInvalid(customer.PhoneNumber), Parameter: nameof(Customer.PhoneNumber)),
                (Rule: IsInvalid(customer.Address), Parameter: nameof(Customer.Address)),
                (Rule: IsInvalid(customer.CreatedDate), Parameter: nameof(Customer.CreatedDate)),
                (Rule: IsInvalid(customer.UpdatedDate), Parameter: nameof(Customer.UpdatedDate)),
                (Rule: IsInvalid(customer.UserId), Parameter: nameof(Customer.UserId)));
        }

        private static void ValidateCustomerNotNull(Customer customer)
        {
            if (customer is null)
            {
                throw new NullCustomerException();
            }
        }

        private static dynamic IsInvalid(Guid Id) => new
        {
            Condition = Id == default,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static void ValidateCustomerId(Guid customerId) =>
            Validate((Rule: IsInvalid(customerId), Parameter: nameof(Customer.Id)));

        private static void ValidateStorageCustomer(Customer maybeCustomer, Guid customerId)
        {
            if (maybeCustomer is null)
            {
                throw new NotFoundCustomerException(customerId);
            }
        }

        private static void ValidateCustomerOnModify(Customer customer)
        {
            ValidateCustomerNotNull(customer);

            Validate(
                (Rule: IsInvalid(customer.Id), Parameter: nameof(Customer.Id)),
                (Rule: IsInvalid(customer.FullName), Parameter: nameof(Customer.FullName)),
                (Rule: IsInvalid(customer.PhoneNumber), Parameter: nameof(Customer.PhoneNumber)),
                (Rule: IsInvalid(customer.Address), Parameter: nameof(Customer.Address)),
                (Rule: IsInvalid(customer.CreatedDate), Parameter: nameof(Customer.CreatedDate)),
                (Rule: IsInvalid(customer.UpdatedDate), Parameter: nameof(Customer.UpdatedDate)),
                (Rule: IsInvalid(customer.UserId), Parameter: nameof(Customer.UserId)));
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidCustomerException = new InvalidCustomerException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidCustomerException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidCustomerException.ThrowIfContainsErrors();
        }
    }
}
