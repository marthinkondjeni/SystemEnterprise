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
        public async Task ShouldThrowValidationExceptionOnAddIfEmployeeIsNullAndLogItAsync()
        {
            // given
            Employee nullEmployee = null;

            var nullEmployeeException =
                new NullEmployeeException();

            var expectedEmployeeValidationException =
                new EmployeeValidationException(nullEmployeeException);

            // when
            ValueTask<Employee> addEmployeeTask =
                this.employeeService.AddEmployeeAsync(nullEmployee);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(() =>
                    addEmployeeTask.AsTask());

            // then
            actualEmployeeValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfEmployeeIsInvalidAndLogItAsync(string invalidText)
        {
            // given
            var invalidEmployee = new Employee
            {
                // TODO:  Add default values for your properties i.e. Name = invalidText
            };

            var invalidEmployeeException =
                new InvalidEmployeeException();

            invalidEmployeeException.AddData(
                key: nameof(Employee.Id),
                values: "Id is required");

            //invalidEmployeeException.AddData(
            //    key: nameof(Employee.Name),
            //    values: "Text is required");

            // TODO: Add or remove data here to suit the validation needs for the Employee model

            invalidEmployeeException.AddData(
                key: nameof(Employee.CreatedDate),
                values: "Date is required");

            invalidEmployeeException.AddData(
                key: nameof(Employee.CreatedByUserId),
                values: "Id is required");

            invalidEmployeeException.AddData(
                key: nameof(Employee.UpdatedDate),
                values: "Date is required");

            invalidEmployeeException.AddData(
                key: nameof(Employee.UpdatedByUserId),
                values: "Id is required");

            var expectedEmployeeValidationException =
                new EmployeeValidationException(invalidEmployeeException);

            // when
            ValueTask<Employee> addEmployeeTask =
                this.employeeService.AddEmployeeAsync(invalidEmployee);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(() =>
                    addEmployeeTask.AsTask());

            // then
            actualEmployeeValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEmployeeAsync(It.IsAny<Employee>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Employee randomEmployee = CreateRandomEmployee(randomDateTimeOffset);
            Employee invalidEmployee = randomEmployee;

            invalidEmployee.UpdatedDate =
                invalidEmployee.CreatedDate.AddDays(randomNumber);

            var invalidEmployeeException = new InvalidEmployeeException();

            invalidEmployeeException.AddData(
                key: nameof(Employee.UpdatedDate),
                values: $"Date is not the same as {nameof(Employee.CreatedDate)}");

            var expectedEmployeeValidationException =
                new EmployeeValidationException(invalidEmployeeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Employee> addEmployeeTask =
                this.employeeService.AddEmployeeAsync(invalidEmployee);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(() =>
                    addEmployeeTask.AsTask());

            // then
            actualEmployeeValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEmployeeAsync(It.IsAny<Employee>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateUserIdsIsNotSameAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Employee randomEmployee = CreateRandomEmployee(randomDateTimeOffset);
            Employee invalidEmployee = randomEmployee;
            invalidEmployee.UpdatedByUserId = Guid.NewGuid();

            var invalidEmployeeException =
                new InvalidEmployeeException();

            invalidEmployeeException.AddData(
                key: nameof(Employee.UpdatedByUserId),
                values: $"Id is not the same as {nameof(Employee.CreatedByUserId)}");

            var expectedEmployeeValidationException =
                new EmployeeValidationException(invalidEmployeeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Employee> addEmployeeTask =
                this.employeeService.AddEmployeeAsync(invalidEmployee);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(() =>
                    addEmployeeTask.AsTask());

            // then
            actualEmployeeValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEmployeeAsync(It.IsAny<Employee>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeOrAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            DateTimeOffset invalidDateTime =
                randomDateTimeOffset.AddMinutes(minutesBeforeOrAfter);

            Employee randomEmployee = CreateRandomEmployee(invalidDateTime);
            Employee invalidEmployee = randomEmployee;

            var invalidEmployeeException =
                new InvalidEmployeeException();

            invalidEmployeeException.AddData(
                key: nameof(Employee.CreatedDate),
                values: "Date is not recent");

            var expectedEmployeeValidationException =
                new EmployeeValidationException(invalidEmployeeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Employee> addEmployeeTask =
                this.employeeService.AddEmployeeAsync(invalidEmployee);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(() =>
                    addEmployeeTask.AsTask());

            // then
            actualEmployeeValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEmployeeAsync(It.IsAny<Employee>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}