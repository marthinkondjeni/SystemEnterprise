using System;
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
        public async Task ShouldRemoveEmployeeByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputEmployeeId = randomId;
            Employee randomEmployee = CreateRandomEmployee();
            Employee storageEmployee = randomEmployee;
            Employee expectedInputEmployee = storageEmployee;
            Employee deletedEmployee = expectedInputEmployee;
            Employee expectedEmployee = deletedEmployee.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(inputEmployeeId))
                    .ReturnsAsync(storageEmployee);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteEmployeeAsync(expectedInputEmployee))
                    .ReturnsAsync(deletedEmployee);

            // when
            Employee actualEmployee = await this.employeeService
                .RemoveEmployeeByIdAsync(inputEmployeeId);

            // then
            actualEmployee.Should().BeEquivalentTo(expectedEmployee);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(inputEmployeeId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEmployeeAsync(expectedInputEmployee),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}