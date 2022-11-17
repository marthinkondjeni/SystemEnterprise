using System;
using Xeptions;

namespace SystemEnterprise.Api.Models.Employees.Exceptions
{
    public class FailedEmployeeServiceException : Xeption
    {
        public FailedEmployeeServiceException(Exception innerException)
            : base(message: "Failed employee service occurred, please contact support", innerException)
        { }
    }
}