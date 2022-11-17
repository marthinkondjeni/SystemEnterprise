using Xeptions;

namespace SystemEnterprise.Api.Models.Employees.Exceptions
{
    public class NullEmployeeException : Xeption
    {
        public NullEmployeeException()
            : base(message: "Employee is null.")
        { }
    }
}