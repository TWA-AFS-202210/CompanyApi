using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using CompanyApi.Model;
using System.Net;
using System.ComponentModel.Design;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : Controller
    {
        private static List<Company> companies = new List<Company>();

        [HttpPost]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            var companyNameExists = companies.Exists(_ => _.Name == company.Name);
            if (companyNameExists)
            {
                return new ConflictResult();
            }

            //  company.ID = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.ID}", company);
        }

        [HttpDelete]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }

        [HttpDelete("{companyID}/employees/{employeeID}")]
        public void DeleteEmployee([FromRoute] string companyID, [FromRoute] string employeeID)
        {
            Company company = companies.Find(item => item.ID == companyID);
            if (company != null)
            {
                var employeeFound = company.Employees.Find(item => item.EmployeeID == employeeID);
                company.Employees.Remove(employeeFound);
            }
        }

        [HttpGet]
        public ActionResult<List<Company>> GetAllCompanies([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            if (pageSize != null && pageIndex != null)
            {
                return companies.Skip((pageIndex.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return companies;
        }

        [HttpGet("{ID}")]
        public ActionResult<Company> GetExistingCompanies(string id)
        {
            var foundcompany = companies.FirstOrDefault(_ => _.ID.Equals(id, StringComparison.Ordinal));
            if (foundcompany == null)
            {
                return new NotFoundResult();
            }

            return foundcompany;
        }

        [HttpGet("{ID}/employees")]
        public ActionResult<List<Employee>> GetAllEmployees([FromRoute] string id)
        {
            Company company = companies.Find(x => x.ID == id);
            if (company != null)
            {
                return company.Employees;
            }

            return BadRequest();
        }

        [HttpPatch]
        [Route("{ID}")]
        public Company ModifyExistingCompanies(Company company)
        {
           var newcompany = companies.FirstOrDefault(_ => _.Name.Equals(_.Name));
           newcompany.Name = company.Name;
           return newcompany;
        }

        [HttpPut("{companyID}/employees/{employeeID}")]
        public ActionResult<Employee> UpdateEmployeeInfo([FromRoute] string companyID, [FromRoute] string employeeID, [FromBody] Employee employee)
        {
            Company company = companies.Find(item => item.ID == companyID);
            if (company != null)
            {
                var employeeFound = company.Employees.Find(item => item.EmployeeID == employeeID);
                employeeFound.EmployeeName = employee.EmployeeName;
                employeeFound.EmployeeSalary = employee.EmployeeSalary;
                return employeeFound;
            }

            return BadRequest();
        }

        [HttpPost("{ID}/employees")]
        public ActionResult<Employee> AddEmployeeToCompany([FromRoute] string? companyID, [FromBody] Employee employee)
        {
            var company = companies.Find(_ => _.ID == companyID);
            if (company == null)
            {
                return NotFound();
            }

            company.Employees.Add(employee);

            return new CreatedResult($"companies/{companyID}/employees/{employee.EmployeeID}", employee);
        }
    }
}
