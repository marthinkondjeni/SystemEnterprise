using System;
using System.Linq;
using System.Threading.Tasks;
using SystemEnterprise.Api.Brokers.DateTimes;
using SystemEnterprise.Api.Brokers.Loggings;
using SystemEnterprise.Api.Brokers.Storages;
using SystemEnterprise.Api.Models.Employees;

namespace SystemEnterprise.Api.Services.Foundations.Employees
{
    public partial class EmployeeService : IEmployeeService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EmployeeService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Employee> AddEmployeeAsync(Employee employee) =>
            TryCatch(async () =>
            {
                ValidateEmployeeOnAdd(employee);

                return await this.storageBroker.InsertEmployeeAsync(employee);
            });

        public IQueryable<Employee> RetrieveAllEmployees() =>
            TryCatch(() => this.storageBroker.SelectAllEmployees());

        public ValueTask<Employee> RetrieveEmployeeByIdAsync(Guid employeeId) =>
            TryCatch(async () =>
            {
                ValidateEmployeeId(employeeId);

                Employee maybeEmployee = await this.storageBroker
                    .SelectEmployeeByIdAsync(employeeId);

                ValidateStorageEmployee(maybeEmployee, employeeId);

                return maybeEmployee;
            });

        public ValueTask<Employee> ModifyEmployeeAsync(Employee employee) =>
            TryCatch(async () =>
            {
                ValidateEmployeeOnModify(employee);

                Employee maybeEmployee =
                    await this.storageBroker.SelectEmployeeByIdAsync(employee.Id);

                ValidateStorageEmployee(maybeEmployee, employee.Id);
                ValidateAgainstStorageEmployeeOnModify(inputEmployee: employee, storageEmployee: maybeEmployee);

                return await this.storageBroker.UpdateEmployeeAsync(employee);
            });

        public ValueTask<Employee> RemoveEmployeeByIdAsync(Guid employeeId) =>
            TryCatch(async () =>
            {
                ValidateEmployeeId(employeeId);

                Employee maybeEmployee = await this.storageBroker
                    .SelectEmployeeByIdAsync(employeeId);

                ValidateStorageEmployee(maybeEmployee, employeeId);

                return await this.storageBroker.DeleteEmployeeAsync(maybeEmployee);
            });
    }
}