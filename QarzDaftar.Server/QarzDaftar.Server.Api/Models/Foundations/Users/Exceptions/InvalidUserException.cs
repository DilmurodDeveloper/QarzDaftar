using Xeptions;

namespace QarzDaftar.Server.Api.Models.Foundations.Users.Exceptions
{
    public class InvalidUserException : Xeption
    {
        public InvalidUserException()
            : base(message: "User is invalid.")
        { }

        public InvalidUserException(string parameterName, object parameterValue)
            : base(message: "User is invalid.")
        {
            AddData(parameterName, parameterValue.ToString());
        }
    }
}
