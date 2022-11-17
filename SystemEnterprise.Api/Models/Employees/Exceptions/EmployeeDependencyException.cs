using Xeptions;

namespace SystemEnterprise.Api.Models.Employees.Exceptions
{
    public class EmployeeDependencyException : Xeption
    {
        public EmployeeDependencyException(Xeption innerException) :
            base(message: "Employee dependency error occurred, contact support.", innerException)
        { }
    }
}