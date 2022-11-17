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
        public async Task ShouldAddEmployeeAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            Employee randomEmployee = CreateRandomEmployee(randomDateTimeOffset);
            Employee inputEmployee = randomEmployee;
            Employee storageEmployee = inputEmployee;
            Employee expectedEmployee = storageEmployee.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertEmployeeAsync(inputEmployee))
                    .ReturnsAsync(storageEmployee);

            // when
            Employee actualEmployee = await this.employeeService
                .AddEmployeeAsync(inputEmployee);

            // then
            actualEmployee.Should().BeEquivalentTo(expectedEmployee);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEmployeeAsync(inputEmployee),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}