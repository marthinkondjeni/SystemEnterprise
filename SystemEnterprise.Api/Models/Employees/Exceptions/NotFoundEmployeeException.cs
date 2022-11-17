using System;
using Xeptions;

namespace SystemEnterprise.Api.Models.Employees.Exceptions
{
    public class NotFoundEmployeeException : Xeption
    {
        public NotFoundEmployeeException(Guid employeeId)
            : base(message: $"Couldn't find employee with employeeId: {employeeId}.")
        { }
    }
}