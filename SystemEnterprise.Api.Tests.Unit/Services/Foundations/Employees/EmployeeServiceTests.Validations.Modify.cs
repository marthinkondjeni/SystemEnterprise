using System;
using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using Moq;
using SystemEnterprise.Api.Models.Employees;
using SystemEnterprise.Api.Models.Employees.Exceptions;
using Xunit;

namespace SystemEnterprise.Api.Tests.Unit.Services.Foundations.Employees
{
    public partial class EmployeeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfEmployeeIsNullAndLogItAsync()
        {
            // given
            Employee nullEmployee = null;
            var nullEmployeeException = new NullEmployeeException();

            var expectedEmployeeValidationException =
                new EmployeeValidationException(nullEmployeeException);

            // when
            ValueTask<Employee> modifyEmployeeTask =
                this.employeeService.ModifyEmployeeAsync(nullEmployee);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(
                    modifyEmployeeTask.AsTask);

            // then
            actualEmployeeValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEmployeeAsync(It.IsAny<Employee>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfEmployeeIsInvalidAndLogItAsync(string invalidText)
        {
            // given 
            var invalidEmployee = new Employee
            {
                // TODO:  Add default values for your properties i.e. Name = invalidText
            };

            var invalidEmployeeException = new InvalidEmployeeException();

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
                values:
                new[] {
                    "Date is required",
                    $"Date is the same as {nameof(Employee.CreatedDate)}"
                });

            invalidEmployeeException.AddData(
                key: nameof(Employee.UpdatedByUserId),
                values: "Id is required");

            var expectedEmployeeValidationException =
                new EmployeeValidationException(invalidEmployeeException);

            // when
            ValueTask<Employee> modifyEmployeeTask =
                this.employeeService.ModifyEmployeeAsync(invalidEmployee);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(
                    modifyEmployeeTask.AsTask);

            //then
            actualEmployeeValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEmployeeAsync(It.IsAny<Employee>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Employee randomEmployee = CreateRandomEmployee(randomDateTimeOffset);
            Employee invalidEmployee = randomEmployee;
            var invalidEmployeeException = new InvalidEmployeeException();

            invalidEmployeeException.AddData(
                key: nameof(Employee.UpdatedDate),
                values: $"Date is the same as {nameof(Employee.CreatedDate)}");

            var expectedEmployeeValidationException =
                new EmployeeValidationException(invalidEmployeeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Employee> modifyEmployeeTask =
                this.employeeService.ModifyEmployeeAsync(invalidEmployee);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(
                    modifyEmployeeTask.AsTask);

            // then
            actualEmployeeValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(invalidEmployee.Id),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Employee randomEmployee = CreateRandomEmployee(randomDateTimeOffset);
            randomEmployee.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidEmployeeException =
                new InvalidEmployeeException();

            invalidEmployeeException.AddData(
                key: nameof(Employee.UpdatedDate),
                values: "Date is not recent");

            var expectedEmployeeValidatonException =
                new EmployeeValidationException(invalidEmployeeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Employee> modifyEmployeeTask =
                this.employeeService.ModifyEmployeeAsync(randomEmployee);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(
                    modifyEmployeeTask.AsTask);

            // then
            actualEmployeeValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidatonException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfEmployeeDoesNotExistAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Employee randomEmployee = CreateRandomModifyEmployee(randomDateTimeOffset);
            Employee nonExistEmployee = randomEmployee;
            Employee nullEmployee = null;

            var notFoundEmployeeException =
                new NotFoundEmployeeException(nonExistEmployee.Id);

            var expectedEmployeeValidationException =
                new EmployeeValidationException(notFoundEmployeeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(nonExistEmployee.Id))
                .ReturnsAsync(nullEmployee);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when 
            ValueTask<Employee> modifyEmployeeTask =
                this.employeeService.ModifyEmployeeAsync(nonExistEmployee);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(
                    modifyEmployeeTask.AsTask);

            // then
            actualEmployeeValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(nonExistEmployee.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNegativeNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Employee randomEmployee = CreateRandomModifyEmployee(randomDateTimeOffset);
            Employee invalidEmployee = randomEmployee.DeepClone();
            Employee storageEmployee = invalidEmployee.DeepClone();
            storageEmployee.CreatedDate = storageEmployee.CreatedDate.AddMinutes(randomMinutes);
            storageEmployee.UpdatedDate = storageEmployee.UpdatedDate.AddMinutes(randomMinutes);
            var invalidEmployeeException = new InvalidEmployeeException();

            invalidEmployeeException.AddData(
                key: nameof(Employee.CreatedDate),
                values: $"Date is not the same as {nameof(Employee.CreatedDate)}");

            var expectedEmployeeValidationException =
                new EmployeeValidationException(invalidEmployeeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(invalidEmployee.Id))
                .ReturnsAsync(storageEmployee);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Employee> modifyEmployeeTask =
                this.employeeService.ModifyEmployeeAsync(invalidEmployee);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(
                    modifyEmployeeTask.AsTask);

            // then
            actualEmployeeValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(invalidEmployee.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedEmployeeValidationException))),
                       Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfCreatedUserIdDontMacthStorageAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Employee randomEmployee = CreateRandomModifyEmployee(randomDateTimeOffset);
            Employee invalidEmployee = randomEmployee.DeepClone();
            Employee storageEmployee = invalidEmployee.DeepClone();
            invalidEmployee.CreatedByUserId = Guid.NewGuid();
            storageEmployee.UpdatedDate = storageEmployee.CreatedDate;

            var invalidEmployeeException = new InvalidEmployeeException();

            invalidEmployeeException.AddData(
                key: nameof(Employee.CreatedByUserId),
                values: $"Id is not the same as {nameof(Employee.CreatedByUserId)}");

            var expectedEmployeeValidationException =
                new EmployeeValidationException(invalidEmployeeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(invalidEmployee.Id))
                .ReturnsAsync(storageEmployee);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Employee> modifyEmployeeTask =
                this.employeeService.ModifyEmployeeAsync(invalidEmployee);

            EmployeeValidationException actualEmployeeValidationException =
                await Assert.ThrowsAsync<EmployeeValidationException>(
                    modifyEmployeeTask.AsTask);

            // then
            actualEmployeeValidationException.Should().BeEquivalentTo(expectedEmployeeValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(invalidEmployee.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedEmployeeValidationException))),
                       Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Employee randomEmployee = CreateRandomModifyEmployee(randomDateTimeOffset);
            Employee invalidEmployee = randomEmployee;
            Employee storageEmployee = randomEmployee.DeepClone();

            var invalidEmployeeException = new InvalidEmployeeException();

            invalidEmployeeException.AddData(
                key: nameof(Employee.UpdatedDate),
                values: $"Date is the same as {nameof(Employee.UpdatedDate)}");

            var expectedEmployeeValidationException =
                new EmployeeValidationException(invalidEmployeeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(invalidEmployee.Id))
                .ReturnsAsync(storageEmployee);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Employee> modifyEmployeeTask =
                this.employeeService.ModifyEmployeeAsync(invalidEmployee);

            // then
            await Assert.ThrowsAsync<EmployeeValidationException>(
                modifyEmployeeTask.AsTask);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(invalidEmployee.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}