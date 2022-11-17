using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemEnterprise.Api.Tests.Acceptance.Brokers;
using SystemEnterprise.Api.Tests.Acceptance.Models.Employees;
using Tynamix.ObjectFiller;
using Xunit;

namespace SystemEnterprise.Api.Tests.Acceptance.Apis.Employees
{
    [Collection(nameof(ApiTestCollection))]
    public partial class EmployeesApiTests
    {
        private readonly ApiBroker apiBroker;

        public EmployeesApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTime() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Employee UpdateEmployeeWithRandomValues(Employee inputEmployee)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<Employee>();

            filler.Setup()
                .OnProperty(employee => employee.Id).Use(inputEmployee.Id)
                .OnType<DateTimeOffset>().Use(GetRandomDateTime())
                .OnProperty(employee => employee.CreatedDate).Use(inputEmployee.CreatedDate)
                .OnProperty(employee => employee.CreatedByUserId).Use(inputEmployee.CreatedByUserId)
                .OnProperty(employee => employee.UpdatedDate).Use(now);

            return filler.Create();
        }

        private async ValueTask<Employee> PostRandomEmployeeAsync()
        {
            Employee randomEmployee = CreateRandomEmployee();
            await this.apiBroker.PostEmployeeAsync(randomEmployee);

            return randomEmployee;
        }

        private async ValueTask<List<Employee>> PostRandomEmployeesAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomEmployees = new List<Employee>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomEmployees.Add(await PostRandomEmployeeAsync());
            }

            return randomEmployees;
        }

        private static Employee CreateRandomEmployee() =>
            CreateRandomEmployeeFiller().Create();

        private static Filler<Employee> CreateRandomEmployeeFiller()
        {
            Guid userId = Guid.NewGuid();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Employee>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(employee => employee.CreatedDate).Use(now)
                .OnProperty(employee => employee.CreatedByUserId).Use(userId)
                .OnProperty(employee => employee.UpdatedDate).Use(now)
                .OnProperty(employee => employee.UpdatedByUserId).Use(userId);

            return filler;
        }
    }
}