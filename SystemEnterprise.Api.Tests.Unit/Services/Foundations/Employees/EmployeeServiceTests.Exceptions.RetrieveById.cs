using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using SystemEnterprise.Api.Models.Employees;
using SystemEnterprise.Api.Models.Employees.Exceptions;
using Xunit;

namespace SystemEnterprise.Api.Tests.Unit.Services.Foundations.Employees
{
    public partial class EmployeeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedEmployeeStorageException =
                new FailedEmployeeStorageException(sqlException);

            var expectedEmployeeDependencyException =
                new EmployeeDependencyException(failedEmployeeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Employee> retrieveEmployeeByIdTask =
                this.employeeService.RetrieveEmployeeByIdAsync(someId);

            EmployeeDependencyException actualEmployeeDependencyException =
                await Assert.ThrowsAsync<EmployeeDependencyException>(
                    retrieveEmployeeByIdTask.AsTask);

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
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedEmployeeServiceException =
                new FailedEmployeeServiceException(serviceException);

            var expectedEmployeeServiceException =
                new EmployeeServiceException(failedEmployeeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEmployeeByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Employee> retrieveEmployeeByIdTask =
                this.employeeService.RetrieveEmployeeByIdAsync(someId);

            EmployeeServiceException actualEmployeeServiceException =
                await Assert.ThrowsAsync<EmployeeServiceException>(
                    retrieveEmployeeByIdTask.AsTask);

            // then
            actualEmployeeServiceException.Should()
                .BeEquivalentTo(expectedEmployeeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEmployeeByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

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