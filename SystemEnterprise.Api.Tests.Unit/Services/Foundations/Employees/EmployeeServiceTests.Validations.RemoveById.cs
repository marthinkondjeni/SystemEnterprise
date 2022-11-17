using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using SystemEnterprise.Api.Models.Employees;
using SystemEnterprise.Api.Models.Employees.Exceptions;
using Xunit;

namespace SystemEnterprise.Api.Tests.Unit.Services.Foundations.Employees
{
    public partial class EmployeeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveIfIdIsInvalidAndLogItAsync()
        {
            // given
            Guid invalidEmployeeId = Guid.Empty;

            var invalidEmployeeException =
                new InvalidEmployeeException();

            invalidEmployeeException.AddData(
                key: nameof(Employee.Id),
                values: "Id is required");

            var expectedEmployeeValidationException =
                new EmployeeValidationException(invalidEmployeeException);

            // when
            ValueTask<Employee> removeEmployeeByIdTask =
                this.employeeService.RemoveEmployeeByIdAsync(invalidEmployeeId);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(
                    removeEmployeeByIdTask.AsTask);

            // then
            actualEmployeeValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEmployeeAsync(It.IsAny<Employee>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}