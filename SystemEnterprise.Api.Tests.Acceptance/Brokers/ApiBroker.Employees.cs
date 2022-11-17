using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemEnterprise.Api.Tests.Acceptance.Models.Employees;

namespace SystemEnterprise.Api.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string EmployeesRelativeUrl = "api/employees";

        public async ValueTask<Employee> PostEmployeeAsync(Employee employee) =>
            await this.apiFactoryClient.PostContentAsync(EmployeesRelativeUrl, employee);

        public async ValueTask<Employee> GetEmployeeByIdAsync(Guid employeeId) =>
            await this.apiFactoryClient.GetContentAsync<Employee>($"{EmployeesRelativeUrl}/{employeeId}");

        public async ValueTask<List<Employee>> GetAllEmployeesAsync() =>
          await this.apiFactoryClient.GetContentAsync<List<Employee>>($"{EmployeesRelativeUrl}/");

        public async ValueTask<Employee> PutEmployeeAsync(Employee employee) =>
            await this.apiFactoryClient.PutContentAsync(EmployeesRelativeUrl, employee);

        public async ValueTask<Employee> DeleteEmployeeByIdAsync(Guid employeeId) =>
            await this.apiFactoryClient.DeleteContentAsync<Employee>($"{EmployeesRelativeUrl}/{employeeId}");
    }
}
