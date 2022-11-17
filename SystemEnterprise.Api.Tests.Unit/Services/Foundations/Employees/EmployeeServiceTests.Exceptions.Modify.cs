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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Employee randomEmployee = CreateRandomEmployee();
            SqlException sqlException = GetSqlException();

            var failedEmployeeStorageException =
                new FailedEmployeeStorageException(sqlException);

            var expectedEmployeeDependencyException =
                new EmployeeDependencyException(failedEmployeeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Employee> modifyEmployeeTask =
                this.employeeService.ModifyEmployeeAsync(randomEmployee);

            EmployeeDependencyException actualEmployeeDependencyException =
                await Assert.ThrowsAsync<EmployeeDependencyException>(
                    modifyEmployeeTask.AsTask);

            // then
            actualEmployeeDependencyException.Should()
                .BeEquivalentTo(expectedEmployeeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(randomEmployee.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedEmployeeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEmployeeAsync(randomEmployee),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            Employee someEmployee = CreateRandomEmployee();
            string randomMessage = GetRandomMessage();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidEmployeeReferenceException =
                new InvalidEmployeeReferenceException(foreignKeyConstraintConflictException);

            EmployeeDependencyValidationException expectedEmployeeDependencyValidationException =
                new EmployeeDependencyValidationException(invalidEmployeeReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<Employee> modifyEmployeeTask =
                this.employeeService.ModifyEmployeeAsync(someEmployee);

            EmployeeDependencyValidationException actualEmployeeDependencyValidationException =
                await Assert.ThrowsAsync<EmployeeDependencyValidationException>(
                    modifyEmployeeTask.AsTask);

            // then
            actualEmployeeDependencyValidationException.Should()
                .BeEquivalentTo(expectedEmployeeDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(someEmployee.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(expectedEmployeeDependencyValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEmployeeAsync(someEmployee),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            Employee randomEmployee = CreateRandomEmployee();
            var databaseUpdateException = new DbUpdateException();

            var failedEmployeeStorageException =
                new FailedEmployeeStorageException(databaseUpdateException);

            var expectedEmployeeDependencyException =
                new EmployeeDependencyException(failedEmployeeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Employee> modifyEmployeeTask =
                this.employeeService.ModifyEmployeeAsync(randomEmployee);

            EmployeeDependencyException actualEmployeeDependencyException =
                await Assert.ThrowsAsync<EmployeeDependencyException>(
                    modifyEmployeeTask.AsTask);

            // then
            actualEmployeeDependencyException.Should()
                .BeEquivalentTo(expectedEmployeeDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(randomEmployee.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEmployeeAsync(randomEmployee),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDbUpdateConcurrencyErrorOccursAndLogAsync()
        {
            // given
            Employee randomEmployee = CreateRandomEmployee();
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedEmployeeException =
                new LockedEmployeeException(databaseUpdateConcurrencyException);

            var expectedEmployeeDependencyValidationException =
                new EmployeeDependencyValidationException(lockedEmployeeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<Employee> modifyEmployeeTask =
                this.employeeService.ModifyEmployeeAsync(randomEmployee);

            EmployeeDependencyValidationException actualEmployeeDependencyValidationException =
                await Assert.ThrowsAsync<EmployeeDependencyValidationException>(
                    modifyEmployeeTask.AsTask);

            // then
            actualEmployeeDependencyValidationException.Should()
                .BeEquivalentTo(expectedEmployeeDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(randomEmployee.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEmployeeAsync(randomEmployee),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Employee randomEmployee = CreateRandomEmployee();
            var serviceException = new Exception();

            var failedEmployeeServiceException =
                new FailedEmployeeServiceException(serviceException);

            var expectedEmployeeServiceException =
                new EmployeeServiceException(failedEmployeeServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Employee> modifyEmployeeTask =
                this.employeeService.ModifyEmployeeAsync(randomEmployee);

            EmployeeServiceException actualEmployeeServiceException =
                await Assert.ThrowsAsync<EmployeeServiceException>(
                    modifyEmployeeTask.AsTask);

            // then
            actualEmployeeServiceException.Should()
                .BeEquivalentTo(expectedEmployeeServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(randomEmployee.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEmployeeAsync(randomEmployee),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}