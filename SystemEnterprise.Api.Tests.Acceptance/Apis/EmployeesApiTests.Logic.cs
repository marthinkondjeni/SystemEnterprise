using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using RESTFulSense.Exceptions;
using SystemEnterprise.Api.Tests.Acceptance.Models.Employees;
using Xunit;

namespace SystemEnterprise.Api.Tests.Acceptance.Apis.Employees
{
    public partial class EmployeesApiTests
    {
        [Fact]
        public async Task ShouldPostEmployeeAsync()
        {
            // given
            Employee randomEmployee = CreateRandomEmployee();
            Employee inputEmployee = randomEmployee;
            Employee expectedEmployee = inputEmployee;

            // when 
            await this.apiBroker.PostEmployeeAsync(inputEmployee);

            Employee actualEmployee =
                await this.apiBroker.GetEmployeeByIdAsync(inputEmployee.Id);

            // then
            actualEmployee.Should().BeEquivalentTo(expectedEmployee);
            await this.apiBroker.DeleteEmployeeByIdAsync(actualEmployee.Id);
        }

        [Fact]
        public async Task ShouldGetAllEmployeesAsync()
        {
            // given
            List<Employee> randomEmployees = await PostRandomEmployeesAsync();
            List<Employee> expectedEmployees = randomEmployees;

            // when
            List<Employee> actualEmployees = await this.apiBroker.GetAllEmployeesAsync();

            // then
            foreach (Employee expectedEmployee in expectedEmployees)
            {
                Employee actualEmployee = actualEmployees.Single(approval => approval.Id == expectedEmployee.Id);
                actualEmployee.Should().BeEquivalentTo(expectedEmployee);
                await this.apiBroker.DeleteEmployeeByIdAsync(actualEmployee.Id);
            }
        }

        [Fact]
        public async Task ShouldGetEmployeeAsync()
        {
            // given
            Employee randomEmployee = await PostRandomEmployeeAsync();
            Employee expectedEmployee = randomEmployee;

            // when
            Employee actualEmployee = await this.apiBroker.GetEmployeeByIdAsync(randomEmployee.Id);

            // then
            actualEmployee.Should().BeEquivalentTo(expectedEmployee);
            await this.apiBroker.DeleteEmployeeByIdAsync(actualEmployee.Id);
        }

        [Fact]
        public async Task ShouldPutEmployeeAsync()
        {
            // given
            Employee randomEmployee = await PostRandomEmployeeAsync();
            Employee modifiedEmployee = UpdateEmployeeWithRandomValues(randomEmployee);

            // when
            await this.apiBroker.PutEmployeeAsync(modifiedEmployee);
            Employee actualEmployee = await this.apiBroker.GetEmployeeByIdAsync(randomEmployee.Id);

            // then
            actualEmployee.Should().BeEquivalentTo(modifiedEmployee);
            await this.apiBroker.DeleteEmployeeByIdAsync(actualEmployee.Id);
        }

        [Fact]
        public async Task ShouldDeleteEmployeeAsync()
        {
            // given
            Employee randomEmployee = await PostRandomEmployeeAsync();
            Employee inputEmployee = randomEmployee;
            Employee expectedEmployee = inputEmployee;

            // when
            Employee deletedEmployee =
                await this.apiBroker.DeleteEmployeeByIdAsync(inputEmployee.Id);

            ValueTask<Employee> getEmployeebyIdTask =
                this.apiBroker.GetEmployeeByIdAsync(inputEmployee.Id);

            // then
            deletedEmployee.Should().BeEquivalentTo(expectedEmployee);

            await Assert.ThrowsAsync<HttpResponseNotFoundException>(() =>
                getEmployeebyIdTask.AsTask());
        }
    }
}