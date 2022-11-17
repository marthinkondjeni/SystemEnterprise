using System;
using Xeptions;

namespace SystemEnterprise.Api.Models.Employees.Exceptions
{
    public class AlreadyExistsEmployeeException : Xeption
    {
        public AlreadyExistsEmployeeException(Exception innerException)
            : base(message: "Employee with the same Id already exists.", innerException)
        { }
    }
}