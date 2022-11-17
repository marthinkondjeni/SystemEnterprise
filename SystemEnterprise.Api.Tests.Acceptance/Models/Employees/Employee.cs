// ---------------------------------------------------------------
// Copyright (c) Marthin K. Thomas. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// ---------------------------------------------------------------

using System;

namespace SystemEnterprise.Api.Tests.Acceptance.Models.Employees
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string NationalId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string PreferredName { get; set; }
        public DateTimeOffset BirthDate { get; set; }
        public string Nationality { get; set; }
        public EmployeeTitle Title { get; set; }
        public EmployeeGender Gender { get; set; }
        public EmployeeStatus Status { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        
    }
}
