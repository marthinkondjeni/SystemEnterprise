using System;
using Xeptions;

namespace SystemEnterprise.Api.Models.Employees.Exceptions
{
    public class LockedEmployeeException : Xeption
    {
        public LockedEmployeeException(Exception innerException)
            : base(message: "Locked employee record exception, please try again later", innerException)
        {
        }
    }
}