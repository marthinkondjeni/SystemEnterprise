using System;
using Xeptions;

namespace SystemEnterprise.Api.Models.Employees.Exceptions
{
    public class InvalidEmployeeReferenceException : Xeption
    {
        public InvalidEmployeeReferenceException(Exception innerException)
            : base(message: "Invalid employee reference error occurred.", innerException) { }
    }
}