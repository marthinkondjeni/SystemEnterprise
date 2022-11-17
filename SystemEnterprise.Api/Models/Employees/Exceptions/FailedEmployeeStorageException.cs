using System;
using Xeptions;

namespace SystemEnterprise.Api.Models.Employees.Exceptions
{
    public class FailedEmployeeStorageException : Xeption
    {
        public FailedEmployeeStorageException(Exception innerException)
            : base(message: "Failed employee storage error occurred, contact support.", innerException)
        { }
    }
}