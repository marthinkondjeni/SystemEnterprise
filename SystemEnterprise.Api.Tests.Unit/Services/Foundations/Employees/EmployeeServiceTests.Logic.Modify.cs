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
        public async Task ShouldModifyEmployeeAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Employee randomEmployee = CreateRandomModifyEmployee(randomDateTimeOffset);
            Employee inputEmployee = randomEmployee;
            Employee storageEmployee = inputEmployee.DeepClone();
            storageEmployee.UpdatedDate = randomEmployee.CreatedDate;
            Employee updatedEmployee = inputEmployee;
            Employee expectedEmployee = updatedEmployee.DeepClone();
            Guid employeeId = inputEmployee.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(employeeId))
                    .ReturnsAsync(storageEmployee);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateEmployeeAsync(inputEmployee))
                    .ReturnsAsync(updatedEmployee);

            // when
            Employee actualEmployee =
                await this.employeeService.ModifyEmployeeAsync(inputEmployee);

            // then
            actualEmployee.Should().BeEquivalentTo(expectedEmployee);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(inputEmployee.Id),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEmployeeAsync(inputEmployee),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}