using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using SystemEnterprise.Api.Models.Employees;
using Xunit;

namespace SystemEnterprise.Api.Tests.Unit.Services.Foundations.Employees
{
    public partial class EmployeeServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEmployeeByIdAsync()
        {
            // given
            Employee randomEmployee = CreateRandomEmployee();
            Employee inputEmployee = randomEmployee;
            Employee storageEmployee = randomEmployee;
            Employee expectedEmployee = storageEmployee.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(inputEmployee.Id))
                    .ReturnsAsync(storageEmployee);

            // when
            Employee actualEmployee =
                await this.employeeService.RetrieveEmployeeByIdAsync(inputEmployee.Id);

            // then
            actualEmployee.Should().BeEquivalentTo(expectedEmployee);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(inputEmployee.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}