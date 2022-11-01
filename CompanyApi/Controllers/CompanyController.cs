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
        public ActionResult<List<Company>> GetAllPets()
        {
            return Ok(companies);
        }

        [HttpGet("{companyId}")]
        public ActionResult<Company> GetPetById(string companyId)
        {
            foreach (var company in companies.Where(company => company.CompanyId == companyId))
            {
                return Ok(company);
            }

            return NotFound();
        }
    }
}
