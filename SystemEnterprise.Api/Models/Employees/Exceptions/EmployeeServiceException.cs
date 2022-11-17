using System;
using Xeptions;

namespace SystemEnterprise.Api.Models.Employees.Exceptions
{
    public class EmployeeServiceException : Xeption
    {
        public EmployeeServiceException(Exception innerException)
            : base(message: "Employee service error occurred, contact support.", innerException)
        { }
    }
}