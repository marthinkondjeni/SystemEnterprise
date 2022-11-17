using Xeptions;

namespace SystemEnterprise.Api.Models.Employees.Exceptions
{
    public class InvalidEmployeeException : Xeption
    {
        public InvalidEmployeeException()
            : base(message: "Invalid employee. Please correct the errors and try again.")
        { }
    }
}