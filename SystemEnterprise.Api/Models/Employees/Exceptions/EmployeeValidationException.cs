using Xeptions;

namespace SystemEnterprise.Api.Models.Employees.Exceptions
{
    public class EmployeeValidationException : Xeption
    {
        public EmployeeValidationException(Xeption innerException)
            : base(message: "Employee validation errors occurred, please try again.",
                  innerException)
        { }
    }
}