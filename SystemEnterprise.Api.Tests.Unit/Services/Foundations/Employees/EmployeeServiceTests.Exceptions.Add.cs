using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using SystemEnterprise.Api.Models.Employees;
using SystemEnterprise.Api.Models.Employees.Exceptions;
using Xunit;

namespace SystemEnterprise.Api.Tests.Unit.Services.Foundations.Employees
{
    public partial class EmployeeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Employee someEmployee = CreateRandomEmployee();
            SqlException sqlException = GetSqlException();

            var failedEmployeeStorageException =
                new FailedEmployeeStorageException(sqlException);

            var expectedEmployeeDependencyException =
                new EmployeeDependencyException(failedEmployeeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Employee> addEmployeeTask =
                this.employeeService.AddEmployeeAsync(someEmployee);

            EmployeeDependencyException actualEmployeeDependencyException =
                await Assert.ThrowsAsync<EmployeeDependencyException>(
                    addEmployeeTask.AsTask);

            // then
            actualEmployeeDependencyException.Should()
                .BeEquivalentTo(expectedEmployeeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEmployeeAsync(It.IsAny<Employee>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedEmployeeDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfEmployeeAlreadyExsitsAndLogItAsync()
        {
            // given
            Employee randomEmployee = CreateRandomEmployee();
            Employee alreadyExistsEmployee = randomEmployee;
            string randomMessage = GetRandomMessage();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsEmployeeException =
                new AlreadyExistsEmployeeException(duplicateKeyException);

            var expectedEmployeeDependencyValidationException =
                new EmployeeDependencyValidationException(alreadyExistsEmployeeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(duplicateKeyException);

            // when
            ValueTask<Employee> addEmployeeTask =
                this.employeeService.AddEmployeeAsync(alreadyExistsEmployee);

            // then
            EmployeeDependencyValidationException actualEmployeeDependencyValidationException =
                await Assert.ThrowsAsync<EmployeeDependencyValidationException>(
                    addEmployeeTask.AsTask);

            actualEmployeeDependencyValidationException.Should()
                .BeEquivalentTo(expectedEmployeeDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEmployeeAsync(It.IsAny<Employee>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            Employee someEmployee = CreateRandomEmployee();
            string randomMessage = GetRandomMessage();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidEmployeeReferenceException =
                new InvalidEmployeeReferenceException(foreignKeyConstraintConflictException);

            var expectedEmployeeValidationException =
                new EmployeeDependencyValidationException(invalidEmployeeReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<Employee> addEmployeeTask =
                this.employeeService.AddEmployeeAsync(someEmployee);

            // then
            EmployeeDependencyValidationException actualEmployeeDependencyValidationException =
                await Assert.ThrowsAsync<EmployeeDependencyValidationException>(
                    addEmployeeTask.AsTask);

            actualEmployeeDependencyValidationException.Should()
                .BeEquivalentTo(expectedEmployeeValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEmployeeAsync(someEmployee),
                    Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Employee someEmployee = CreateRandomEmployee();

            var databaseUpdateException =
                new DbUpdateException();

            var failedEmployeeStorageException =
                new FailedEmployeeStorageException(databaseUpdateException);

            var expectedEmployeeDependencyException =
                new EmployeeDependencyException(failedEmployeeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Employee> addEmployeeTask =
                this.employeeService.AddEmployeeAsync(someEmployee);

            EmployeeDependencyException actualEmployeeDependencyException =
                await Assert.ThrowsAsync<EmployeeDependencyException>(
                    addEmployeeTask.AsTask);

            // then
            actualEmployeeDependencyException.Should()
                .BeEquivalentTo(expectedEmployeeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEmployeeAsync(It.IsAny<Employee>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Employee someEmployee = CreateRandomEmployee();
            var serviceException = new Exception();

            var failedEmployeeServiceException =
                new FailedEmployeeServiceException(serviceException);

            var expectedEmployeeServiceException =
                new EmployeeServiceException(failedEmployeeServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Employee> addEmployeeTask =
                this.employeeService.AddEmployeeAsync(someEmployee);

            EmployeeServiceException actualEmployeeServiceException =
                await Assert.ThrowsAsync<EmployeeServiceException>(
                    addEmployeeTask.AsTask);

            // then
            actualEmployeeServiceException.Should()
                .BeEquivalentTo(expectedEmployeeServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEmployeeAsync(It.IsAny<Employee>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}