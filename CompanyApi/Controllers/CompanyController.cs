using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();
        [HttpPost]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            if (companies.Any(existCompany => existCompany.Name == company.Name))
            {
                return Conflict();
            }

            company.CompanyId = Guid.NewGuid().ToString();

            companies.Add(company);
            return new CreatedResult($"/companies/{company.CompanyId}", company);
        }

        [HttpDelete]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }

        [HttpGet]
        public ActionResult<List<Company>> GetAllCompanies([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            if (pageSize != null && pageIndex != null)
            {
                // return companies.GetRange((pageIndex.Value - 1) * pageSize.Value, pageSize.Value);
                return companies
                    .Skip((pageIndex.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value)
                    .ToList();
            }

            return Ok(companies);
        }

        [HttpGet("{companyId}")]
        public ActionResult<Company> GetCompanyById(string companyId)
        {
            foreach (var company in companies.Where(company => company.CompanyId == companyId))
            {
                return Ok(company);
            }

            return NotFound();
        }

        [HttpPut("{companyId}")]
        public ActionResult<Company> UpdateCompanyBasicInformationById(string companyId, Company company)
        {
            foreach (var existCompany in companies.Where(existCompany => existCompany.CompanyId == companyId))
            {
                existCompany.Name = company.Name;
                return Ok(existCompany);
            }

            return NotFound();
        }

        [HttpPost("{companyId}")]
        public ActionResult<Employee> AddAnEmployeeToSpecificCompany(string companyId, Employee employee)
        {
            foreach (var company in companies.Where(company => company.CompanyId == companyId))
            {
                employee.EmployeeId = Guid.NewGuid().ToString();
                company.Employee = employee;
                return Ok(employee);
            }

            return NotFound();
        }
    }
}
