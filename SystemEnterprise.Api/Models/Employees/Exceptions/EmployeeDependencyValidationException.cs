using Xeptions;

namespace SystemEnterprise.Api.Models.Employees.Exceptions
{
    public class EmployeeDependencyValidationException : Xeption
    {
        public EmployeeDependencyValidationException(Xeption innerException)
            : base(message: "Employee dependency validation occurred, please try again.", innerException)
        { }
    }
}