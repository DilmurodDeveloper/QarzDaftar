using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins;
using QarzDaftar.Server.Api.Models.Foundations.SuperAdmins.Exceptions;

namespace QarzDaftar.Server.Api.Services.Foundations.SuperAdmins
{
    public partial class SuperAdminService
    {
        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Username is required"
        };

        private void ValidateSuperAdminUsername(string username) =>
            Validate((Rule: IsInvalid(username), Parameter: nameof(SuperAdmin.Username)));

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidSuperAdminException = new InvalidSuperAdminException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidSuperAdminException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidSuperAdminException.ThrowIfContainsErrors();
        }
    }
}
