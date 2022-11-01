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
        public List<Company> GetAllCompanies([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            return companies;
        }

        [HttpGet]
        [Route("{ID}")]
        public ActionResult<Company> GetExistingCompanies(string id)
        {
            var foundcompany = companies.FirstOrDefault(_ => _.ID.Equals(id, StringComparison.Ordinal));
            if (foundcompany == null)
            {
                return new NotFoundResult();
            }

            return foundcompany;
        }

        [HttpPatch]
        [Route("{ID}")]
        public ActionResult<Company> ModifyExistingCompanies(Company company)
        {
           var newcompany = companies.FirstOrDefault(_ => _.Name.Equals(_.Name));
           newcompany.Name = company.Name;
           return newcompany;
        }
    }
}
