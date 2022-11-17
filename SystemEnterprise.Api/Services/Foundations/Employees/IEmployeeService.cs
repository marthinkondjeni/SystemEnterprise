using System;
using System.Linq;
using System.Threading.Tasks;
using SystemEnterprise.Api.Models.Employees;

namespace SystemEnterprise.Api.Services.Foundations.Employees
{
    public interface IEmployeeService
    {
        ValueTask<Employee> AddEmployeeAsync(Employee employee);
        IQueryable<Employee> RetrieveAllEmployees();
        ValueTask<Employee> RetrieveEmployeeByIdAsync(Guid employeeId);
        ValueTask<Employee> ModifyEmployeeAsync(Employee employee);
        ValueTask<Employee> RemoveEmployeeByIdAsync(Guid employeeId);
    }
}