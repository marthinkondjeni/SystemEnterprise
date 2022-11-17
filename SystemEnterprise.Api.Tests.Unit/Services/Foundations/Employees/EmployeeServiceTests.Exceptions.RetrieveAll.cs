using System;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using SystemEnterprise.Api.Models.Employees.Exceptions;
using Xunit;

namespace SystemEnterprise.Api.Tests.Unit.Services.Foundations.Employees
{
    public partial class EmployeeServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedStorageException =
                new FailedEmployeeStorageException(sqlException);

            var expectedEmployeeDependencyException =
                new EmployeeDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEmployees())
                    .Throws(sqlException);

            // when
            Action retrieveAllEmployeesAction = () =>
                this.employeeService.RetrieveAllEmployees();

            EmployeeDependencyException actualEmployeeDependencyException =
                Assert.Throws<EmployeeDependencyException>(retrieveAllEmployeesAction);

            // then
            actualEmployeeDependencyException.Should()
                .BeEquivalentTo(expectedEmployeeDependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEmployees(),
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
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomMessage();
            var serviceException = new Exception(exceptionMessage);

            var failedEmployeeServiceException =
                new FailedEmployeeServiceException(serviceException);

            var expectedEmployeeServiceException =
                new EmployeeServiceException(failedEmployeeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEmployees())
                    .Throws(serviceException);

            // when
            Action retrieveAllEmployeesAction = () =>
                this.employeeService.RetrieveAllEmployees();

            EmployeeServiceException actualEmployeeServiceException =
                Assert.Throws<EmployeeServiceException>(retrieveAllEmployeesAction);

            // then
            actualEmployeeServiceException.Should()
                .BeEquivalentTo(expectedEmployeeServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEmployees(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedEmployeeServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}