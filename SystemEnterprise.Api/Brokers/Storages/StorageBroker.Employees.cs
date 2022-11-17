using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SystemEnterprise.Api.Models.Employees;

namespace SystemEnterprise.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Employee> Employees { get; set; }

        public async ValueTask<Employee> InsertEmployeeAsync(Employee employee)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Employee> employeeEntityEntry =
                await broker.Employees.AddAsync(employee);

            await broker.SaveChangesAsync();

            return employeeEntityEntry.Entity;
        }

        public IQueryable<Employee> SelectAllEmployees()
        {
            using var broker =
                new StorageBroker(this.configuration);

            return broker.Employees;
        }

        public async ValueTask<Employee> SelectEmployeeByIdAsync(Guid employeeId)
        {
            using var broker =
                new StorageBroker(this.configuration);

            return await broker.Employees.FindAsync(employeeId);
        }

        public async ValueTask<Employee> UpdateEmployeeAsync(Employee employee)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Employee> employeeEntityEntry =
                broker.Employees.Update(employee);

            await broker.SaveChangesAsync();

            return employeeEntityEntry.Entity;
        }

        public async ValueTask<Employee> DeleteEmployeeAsync(Employee employee)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Employee> employeeEntityEntry =
                broker.Employees.Remove(employee);

            await broker.SaveChangesAsync();

            return employeeEntityEntry.Entity;
        }
    }
}
