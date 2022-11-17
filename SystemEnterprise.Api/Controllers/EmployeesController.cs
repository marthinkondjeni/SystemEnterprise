using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;
using SystemEnterprise.Api.Models.Employees;
using SystemEnterprise.Api.Models.Employees.Exceptions;
using SystemEnterprise.Api.Services.Foundations.Employees;

namespace SystemEnterprise.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : RESTFulController
    {
        private readonly IEmployeeService employeeService;

        public EmployeesController(IEmployeeService employeeService) =>
            this.employeeService = employeeService;

        [HttpPost]
        public async ValueTask<ActionResult<Employee>> PostEmployeeAsync(Employee employee)
        {
            try
            {
                Employee addedEmployee =
                    await this.employeeService.AddEmployeeAsync(employee);

                return Created(addedEmployee);
            }
            catch (EmployeeValidationException employeeValidationException)
            {
                return BadRequest(employeeValidationException.InnerException);
            }
            catch (EmployeeDependencyValidationException employeeValidationException)
                when (employeeValidationException.InnerException is InvalidEmployeeReferenceException)
            {
                return FailedDependency(employeeValidationException.InnerException);
            }
            catch (EmployeeDependencyValidationException employeeDependencyValidationException)
               when (employeeDependencyValidationException.InnerException is AlreadyExistsEmployeeException)
            {
                return Conflict(employeeDependencyValidationException.InnerException);
            }
            catch (EmployeeDependencyException employeeDependencyException)
            {
                return InternalServerError(employeeDependencyException);
            }
            catch (EmployeeServiceException employeeServiceException)
            {
                return InternalServerError(employeeServiceException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Employee>> GetAllEmployees()
        {
            try
            {
                IQueryable<Employee> retrievedEmployees =
                    this.employeeService.RetrieveAllEmployees();

                return Ok(retrievedEmployees);
            }
            catch (EmployeeDependencyException employeeDependencyException)
            {
                return InternalServerError(employeeDependencyException);
            }
            catch (EmployeeServiceException employeeServiceException)
            {
                return InternalServerError(employeeServiceException);
            }
        }

        [HttpGet("{employeeId}")]
        public async ValueTask<ActionResult<Employee>> GetEmployeeByIdAsync(Guid employeeId)
        {
            try
            {
                Employee employee = await this.employeeService.RetrieveEmployeeByIdAsync(employeeId);

                return Ok(employee);
            }
            catch (EmployeeValidationException employeeValidationException)
                when (employeeValidationException.InnerException is NotFoundEmployeeException)
            {
                return NotFound(employeeValidationException.InnerException);
            }
            catch (EmployeeValidationException employeeValidationException)
            {
                return BadRequest(employeeValidationException.InnerException);
            }
            catch (EmployeeDependencyException employeeDependencyException)
            {
                return InternalServerError(employeeDependencyException);
            }
            catch (EmployeeServiceException employeeServiceException)
            {
                return InternalServerError(employeeServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Employee>> PutEmployeeAsync(Employee employee)
        {
            try
            {
                Employee modifiedEmployee =
                    await this.employeeService.ModifyEmployeeAsync(employee);

                return Ok(modifiedEmployee);
            }
            catch (EmployeeValidationException employeeValidationException)
                when (employeeValidationException.InnerException is NotFoundEmployeeException)
            {
                return NotFound(employeeValidationException.InnerException);
            }
            catch (EmployeeValidationException employeeValidationException)
            {
                return BadRequest(employeeValidationException.InnerException);
            }
            catch (EmployeeDependencyValidationException employeeValidationException)
                when (employeeValidationException.InnerException is InvalidEmployeeReferenceException)
            {
                return FailedDependency(employeeValidationException.InnerException);
            }
            catch (EmployeeDependencyValidationException employeeDependencyValidationException)
               when (employeeDependencyValidationException.InnerException is AlreadyExistsEmployeeException)
            {
                return Conflict(employeeDependencyValidationException.InnerException);
            }
            catch (EmployeeDependencyException employeeDependencyException)
            {
                return InternalServerError(employeeDependencyException);
            }
            catch (EmployeeServiceException employeeServiceException)
            {
                return InternalServerError(employeeServiceException);
            }
        }

        [HttpDelete("{employeeId}")]
        public async ValueTask<ActionResult<Employee>> DeleteEmployeeByIdAsync(Guid employeeId)
        {
            try
            {
                Employee deletedEmployee =
                    await this.employeeService.RemoveEmployeeByIdAsync(employeeId);

                return Ok(deletedEmployee);
            }
            catch (EmployeeValidationException employeeValidationException)
                when (employeeValidationException.InnerException is NotFoundEmployeeException)
            {
                return NotFound(employeeValidationException.InnerException);
            }
            catch (EmployeeValidationException employeeValidationException)
            {
                return BadRequest(employeeValidationException.InnerException);
            }
            catch (EmployeeDependencyValidationException employeeDependencyValidationException)
                when (employeeDependencyValidationException.InnerException is LockedEmployeeException)
            {
                return Locked(employeeDependencyValidationException.InnerException);
            }
            catch (EmployeeDependencyValidationException employeeDependencyValidationException)
            {
                return BadRequest(employeeDependencyValidationException);
            }
            catch (EmployeeDependencyException employeeDependencyException)
            {
                return InternalServerError(employeeDependencyException);
            }
            catch (EmployeeServiceException employeeServiceException)
            {
                return InternalServerError(employeeServiceException);
            }
        }
    }
}