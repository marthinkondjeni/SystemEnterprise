using System;
using SystemEnterprise.Api.Models.Employees;
using SystemEnterprise.Api.Models.Employees.Exceptions;

namespace SystemEnterprise.Api.Services.Foundations.Employees
{
    public partial class EmployeeService
    {
        private void ValidateEmployeeOnAdd(Employee employee)
        {
            ValidateEmployeeIsNotNull(employee);

            Validate(
                (Rule: IsInvalid(employee.Id), Parameter: nameof(Employee.Id)),

                // TODO: Add any other required validation rules

                (Rule: IsInvalid(employee.CreatedDate), Parameter: nameof(Employee.CreatedDate)),
                (Rule: IsInvalid(employee.CreatedByUserId), Parameter: nameof(Employee.CreatedByUserId)),
                (Rule: IsInvalid(employee.UpdatedDate), Parameter: nameof(Employee.UpdatedDate)),
                (Rule: IsInvalid(employee.UpdatedByUserId), Parameter: nameof(Employee.UpdatedByUserId)),

                (Rule: IsNotSame(
                    firstDate: employee.UpdatedDate,
                    secondDate: employee.CreatedDate,
                    secondDateName: nameof(Employee.CreatedDate)),
                Parameter: nameof(Employee.UpdatedDate)),

                (Rule: IsNotSame(
                    firstId: employee.UpdatedByUserId,
                    secondId: employee.CreatedByUserId,
                    secondIdName: nameof(Employee.CreatedByUserId)),
                Parameter: nameof(Employee.UpdatedByUserId)),

                (Rule: IsNotRecent(employee.CreatedDate), Parameter: nameof(Employee.CreatedDate)));
        }

        private void ValidateEmployeeOnModify(Employee employee)
        {
            ValidateEmployeeIsNotNull(employee);

            Validate(
                (Rule: IsInvalid(employee.Id), Parameter: nameof(Employee.Id)),

                // TODO: Add any other required validation rules

                (Rule: IsInvalid(employee.CreatedDate), Parameter: nameof(Employee.CreatedDate)),
                (Rule: IsInvalid(employee.CreatedByUserId), Parameter: nameof(Employee.CreatedByUserId)),
                (Rule: IsInvalid(employee.UpdatedDate), Parameter: nameof(Employee.UpdatedDate)),
                (Rule: IsInvalid(employee.UpdatedByUserId), Parameter: nameof(Employee.UpdatedByUserId)),

                (Rule: IsSame(
                    firstDate: employee.UpdatedDate,
                    secondDate: employee.CreatedDate,
                    secondDateName: nameof(Employee.CreatedDate)),
                Parameter: nameof(Employee.UpdatedDate)),

                (Rule: IsNotRecent(employee.UpdatedDate), Parameter: nameof(employee.UpdatedDate)));
        }

        public void ValidateEmployeeId(Guid employeeId) =>
            Validate((Rule: IsInvalid(employeeId), Parameter: nameof(Employee.Id)));

        private static void ValidateStorageEmployee(Employee maybeEmployee, Guid employeeId)
        {
            if (maybeEmployee is null)
            {
                throw new NotFoundEmployeeException(employeeId);
            }
        }

        private static void ValidateEmployeeIsNotNull(Employee employee)
        {
            if (employee is null)
            {
                throw new NullEmployeeException();
            }
        }

        private static void ValidateAgainstStorageEmployeeOnModify(Employee inputEmployee, Employee storageEmployee)
        {
            Validate(
                (Rule: IsNotSame(
                    firstDate: inputEmployee.CreatedDate,
                    secondDate: storageEmployee.CreatedDate,
                    secondDateName: nameof(Employee.CreatedDate)),
                Parameter: nameof(Employee.CreatedDate)),

                (Rule: IsNotSame(
                    firstId: inputEmployee.CreatedByUserId,
                    secondId: storageEmployee.CreatedByUserId,
                    secondIdName: nameof(Employee.CreatedByUserId)),
                Parameter: nameof(Employee.CreatedByUserId)),

                (Rule: IsSame(
                    firstDate: inputEmployee.UpdatedDate,
                    secondDate: storageEmployee.UpdatedDate,
                    secondDateName: nameof(Employee.UpdatedDate)),
                Parameter: nameof(Employee.UpdatedDate)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            Guid firstId,
            Guid secondId,
            string secondIdName) => new
            {
                Condition = firstId != secondId,
                Message = $"Id is not the same as {secondIdName}"
            };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                this.dateTimeBroker.GetCurrentDateTimeOffset();

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEmployeeException = new InvalidEmployeeException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEmployeeException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEmployeeException.ThrowIfContainsErrors();
        }
    }
}