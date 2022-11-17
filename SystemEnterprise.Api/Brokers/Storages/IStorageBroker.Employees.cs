using System;
using System.Linq;
using System.Threading.Tasks;
using SystemEnterprise.Api.Models.Employees;

namespace SystemEnterprise.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Employee> InsertEmployeeAsync(Employee employee);
        IQueryable<Employee> SelectAllEmployees();
        ValueTask<Employee> SelectEmployeeByIdAsync(Guid employeeId);
        ValueTask<Employee> UpdateEmployeeAsync(Employee employee);
        ValueTask<Employee> DeleteEmployeeAsync(Employee employee);
    }
}
