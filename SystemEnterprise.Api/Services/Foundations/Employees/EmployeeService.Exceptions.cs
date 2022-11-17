using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SystemEnterprise.Api.Models.Employees;
using SystemEnterprise.Api.Models.Employees.Exceptions;
using Xeptions;

namespace SystemEnterprise.Api.Services.Foundations.Employees
{
    public partial class EmployeeService
    {
        private delegate ValueTask<Employee> ReturningEmployeeFunction();
        private delegate IQueryable<Employee> ReturningEmployeesFunction();

        private async ValueTask<Employee> TryCatch(ReturningEmployeeFunction returningEmployeeFunction)
        {
            try
            {
                return await returningEmployeeFunction();
            }
            catch (NullEmployeeException nullEmployeeException)
            {
                throw CreateAndLogValidationException(nullEmployeeException);
            }
            catch (InvalidEmployeeException invalidEmployeeException)
            {
                throw CreateAndLogValidationException(invalidEmployeeException);
            }
            catch (SqlException sqlException)
            {
                var failedEmployeeStorageException =
                    new FailedEmployeeStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedEmployeeStorageException);
            }
            catch (NotFoundEmployeeException notFoundEmployeeException)
            {
                throw CreateAndLogValidationException(notFoundEmployeeException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsEmployeeException =
                    new AlreadyExistsEmployeeException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsEmployeeException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidEmployeeReferenceException =
                    new InvalidEmployeeReferenceException(foreignKeyConstraintConflictException);

                throw CreateAndLogDependencyValidationException(invalidEmployeeReferenceException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedEmployeeException = new LockedEmployeeException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedEmployeeException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedEmployeeStorageException =
                    new FailedEmployeeStorageException(databaseUpdateException);

                throw CreateAndLogDependencyException(failedEmployeeStorageException);
            }
            catch (Exception exception)
            {
                var failedEmployeeServiceException =
                    new FailedEmployeeServiceException(exception);

                throw CreateAndLogServiceException(failedEmployeeServiceException);
            }
        }

        private IQueryable<Employee> TryCatch(ReturningEmployeesFunction returningEmployeesFunction)
        {
            try
            {
                return returningEmployeesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedEmployeeStorageException =
                    new FailedEmployeeStorageException(sqlException);
                throw CreateAndLogCriticalDependencyException(failedEmployeeStorageException);
            }
            catch (Exception exception)
            {
                var failedEmployeeServiceException =
                    new FailedEmployeeServiceException(exception);

                throw CreateAndLogServiceException(failedEmployeeServiceException);
            }
        }

        private EmployeeValidationException CreateAndLogValidationException(Xeption exception)
        {
            var employeeValidationException =
                new EmployeeValidationException(exception);

            this.loggingBroker.LogError(employeeValidationException);

            return employeeValidationException;
        }

        private EmployeeDependencyException CreateAndLogCriticalDependencyException(Xeption exception)
        {
            var employeeDependencyException = new EmployeeDependencyException(exception);
            this.loggingBroker.LogCritical(employeeDependencyException);

            return employeeDependencyException;
        }

        private EmployeeDependencyValidationException CreateAndLogDependencyValidationException(Xeption exception)
        {
            var employeeDependencyValidationException =
                new EmployeeDependencyValidationException(exception);

            this.loggingBroker.LogError(employeeDependencyValidationException);

            return employeeDependencyValidationException;
        }

        private EmployeeDependencyException CreateAndLogDependencyException(
            Xeption exception)
        {
            var employeeDependencyException = new EmployeeDependencyException(exception);
            this.loggingBroker.LogError(employeeDependencyException);

            return employeeDependencyException;
        }

        private EmployeeServiceException CreateAndLogServiceException(
            Xeption exception)
        {
            var employeeServiceException = new EmployeeServiceException(exception);
            this.loggingBroker.LogError(employeeServiceException);

            return employeeServiceException;
        }
    }
}