using System.Linq;
using FluentAssertions;
using Moq;
using SystemEnterprise.Api.Models.Employees;
using Xunit;

namespace SystemEnterprise.Api.Tests.Unit.Services.Foundations.Employees
{
    public partial class EmployeeServiceTests
    {
        [Fact]
        public void ShouldReturnEmployees()
        {
            // given
            IQueryable<Employee> randomEmployees = CreateRandomEmployees();
            IQueryable<Employee> storageEmployees = randomEmployees;
            IQueryable<Employee> expectedEmployees = storageEmployees;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEmployees())
                    .Returns(storageEmployees);

            // when
            IQueryable<Employee> actualEmployees =
                this.employeeService.RetrieveAllEmployees();

            // then
            actualEmployees.Should().BeEquivalentTo(expectedEmployees);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEmployees(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}