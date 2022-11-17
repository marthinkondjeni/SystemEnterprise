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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidEmployeeId = Guid.Empty;

            var invalidEmployeeException =
                new InvalidEmployeeException();

            invalidEmployeeException.AddData(
                key: nameof(Employee.Id),
                values: "Id is required");

            var expectedEmployeeValidationException =
                new EmployeeValidationException(invalidEmployeeException);

            // when
            ValueTask<Employee> retrieveEmployeeByIdTask =
                this.employeeService.RetrieveEmployeeByIdAsync(invalidEmployeeId);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(
                    retrieveEmployeeByIdTask.AsTask);

            // then
            actualEmployeeValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfEmployeeIsNotFoundAndLogItAsync()
        {
            //given
            Guid someEmployeeId = Guid.NewGuid();
            Employee noEmployee = null;

            var notFoundEmployeeException =
                new NotFoundEmployeeException(someEmployeeId);

            var expectedEmployeeValidationException =
                new EmployeeValidationException(notFoundEmployeeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noEmployee);

            //when
            ValueTask<Employee> retrieveEmployeeByIdTask =
                this.employeeService.RetrieveEmployeeByIdAsync(someEmployeeId);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(
                    retrieveEmployeeByIdTask.AsTask);

            //then
            actualEmployeeValidationException.Should().BeEquivalentTo(expectedEmployeeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}