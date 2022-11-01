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
        private readonly List<Company> companies = new List<Company>();

        [HttpPost]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            company.ID = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.ID}", company);
        }
    }
}
