using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using CompanyApi.Model;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController
    {
        private static readonly List<Company> companies = new List<Company>();

        [HttpPost]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            var companyNameExists = companies.Exists(_ => _.Name == company.Name);
            if (companyNameExists)
            {
                return new ConflictResult();
            }

            company.ID = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.ID}", company);
        }

        [HttpDelete]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }

        [HttpGet]
        public ActionResult<List<Company>> GetAllCompanies()
        {
            return companies;
        }

        [HttpGet]
        [Route("{ID}")]
        public ActionResult<Company> GetExistingCompanies(string id)
        {
            return companies.FirstOrDefault(_ => _.ID.Equals(id, StringComparison.Ordinal));
        }
    }
}
