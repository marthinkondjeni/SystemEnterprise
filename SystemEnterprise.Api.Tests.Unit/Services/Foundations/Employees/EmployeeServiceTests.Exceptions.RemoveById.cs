using System;
using System.Threading.Tasks;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Employee randomEmployee = CreateRandomEmployee();
            SqlException sqlException = GetSqlException();

            var failedEmployeeStorageException =
                new FailedEmployeeStorageException(sqlException);

            var expectedEmployeeDependencyException =
                new EmployeeDependencyException(failedEmployeeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(randomEmployee.Id))
                    .Throws(sqlException);

            // when
            ValueTask<Employee> addEmployeeTask =
                this.employeeService.RemoveEmployeeByIdAsync(randomEmployee.Id);

            EmployeeDependencyException actualEmployeeDependencyException =
                await Assert.ThrowsAsync<EmployeeDependencyException>(
                    addEmployeeTask.AsTask);

            // then
            actualEmployeeDependencyException.Should()
                .BeEquivalentTo(expectedEmployeeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(randomEmployee.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedEmployeeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEmployeeAsync(It.IsAny<Employee>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someEmployeeId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedEmployeeException =
                new LockedEmployeeException(databaseUpdateConcurrencyException);

            var expectedEmployeeDependencyValidationException =
                new EmployeeDependencyValidationException(lockedEmployeeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Employee> removeEmployeeByIdTask =
                this.employeeService.RemoveEmployeeByIdAsync(someEmployeeId);

            EmployeeDependencyValidationException actualEmployeeDependencyValidationException =
                await Assert.ThrowsAsync<EmployeeDependencyValidationException>(
                    removeEmployeeByIdTask.AsTask);

            // then
            actualEmployeeDependencyValidationException.Should()
                .BeEquivalentTo(expectedEmployeeDependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEmployeeAsync(It.IsAny<Employee>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someEmployeeId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedEmployeeStorageException =
                new FailedEmployeeStorageException(sqlException);

            var expectedEmployeeDependencyException =
                new EmployeeDependencyException(failedEmployeeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Employee> deleteEmployeeTask =
                this.employeeService.RemoveEmployeeByIdAsync(someEmployeeId);

            EmployeeDependencyException actualEmployeeDependencyException =
                await Assert.ThrowsAsync<EmployeeDependencyException>(
                    deleteEmployeeTask.AsTask);

            // then
            actualEmployeeDependencyException.Should()
                .BeEquivalentTo(expectedEmployeeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedEmployeeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someEmployeeId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedEmployeeServiceException =
                new FailedEmployeeServiceException(serviceException);

            var expectedEmployeeServiceException =
                new EmployeeServiceException(failedEmployeeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Employee> removeEmployeeByIdTask =
                this.employeeService.RemoveEmployeeByIdAsync(someEmployeeId);

            EmployeeServiceException actualEmployeeServiceException =
                await Assert.ThrowsAsync<EmployeeServiceException>(
                    removeEmployeeByIdTask.AsTask);

            // then
            actualEmployeeServiceException.Should()
                .BeEquivalentTo(expectedEmployeeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(It.IsAny<Guid>()),
                        Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}