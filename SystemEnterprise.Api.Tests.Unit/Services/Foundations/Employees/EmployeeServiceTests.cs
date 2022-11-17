using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.Data.SqlClient;
using Moq;
using SystemEnterprise.Api.Brokers.DateTimes;
using SystemEnterprise.Api.Brokers.Loggings;
using SystemEnterprise.Api.Brokers.Storages;
using SystemEnterprise.Api.Models.Employees;
using SystemEnterprise.Api.Services.Foundations.Employees;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace SystemEnterprise.Api.Tests.Unit.Services.Foundations.Employees
{
    public partial class EmployeeServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEmployeeService employeeService;

        public EmployeeServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.employeeService = new EmployeeService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomMessage() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        public static TheoryData MinutesBeforeOrAfter()
        {
            int randomNumber = GetRandomNumber();
            int randomNegativeNumber = GetRandomNegativeNumber();

            return new TheoryData<int>
            {
                randomNumber,
                randomNegativeNumber
            };
        }

        private static SqlException GetSqlException() =>
            (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Employee CreateRandomModifyEmployee(DateTimeOffset dateTimeOffset)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            Employee randomEmployee = CreateRandomEmployee(dateTimeOffset);

            randomEmployee.CreatedDate =
                randomEmployee.CreatedDate.AddDays(randomDaysInPast);

            return randomEmployee;
        }

        private static IQueryable<Employee> CreateRandomEmployees()
        {
            return CreateEmployeeFiller(dateTimeOffset: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Employee CreateRandomEmployee() =>
            CreateEmployeeFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static Employee CreateRandomEmployee(DateTimeOffset dateTimeOffset) =>
            CreateEmployeeFiller(dateTimeOffset).Create();

        private static Filler<Employee> CreateEmployeeFiller(DateTimeOffset dateTimeOffset)
        {
            Guid userId = Guid.NewGuid();
            var filler = new Filler<Employee>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(employee => employee.CreatedByUserId).Use(userId)
                .OnProperty(employee => employee.UpdatedByUserId).Use(userId);

            // TODO: Complete the filler setup e.g. ignore related properties etc...

            return filler;
        }
    }
}