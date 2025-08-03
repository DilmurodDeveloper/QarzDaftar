using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Debts.Exceptions
{
    public class FailedDebtStorageException : Xeption
    {
        public FailedDebtStorageException(Exception innerException)
            : base(message: "Failed debt storage error occurred, contact support.",
                  innerException)
        { }
    }
}
