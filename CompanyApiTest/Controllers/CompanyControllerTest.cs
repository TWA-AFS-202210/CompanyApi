using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Xunit.Sdk;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async void Should_add_new_company_successfully()
        {
            /*
             * 1. Create Application
             * 2. Create HttpClient
             * 3. Prepare request body (serializeToJson, SerializeToHttpContent)
             * 4. Call API
             * 5. Verify status code
             * 6. Verify response body (DeSerializeToObject)
             */
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");

            // when
            var response = await httpClient.PostAsync("/companies", postBody);

            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotEmpty(createdCompany.CompanyId);
        }

        [Fact]
        public async void Should_return_409_when_add_exist_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies", postBody);

            // when
            var response = await httpClient.PostAsync("/companies", postBody);

            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async void Should_get_all_companies_successfully()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var companyList = new List<Company>()
            {
                new Company(name: "SLB"),
                new Company(name: "TW"),
            };

            foreach (var company in companyList)
            {
                var companyJson = JsonConvert.SerializeObject(company);
                var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
                await httpClient.PostAsync("/companies", postBody);
            }

            //when
            var responseList = await httpClient.GetAsync("/companies");

            // then
            responseList.EnsureSuccessStatusCode();
            var responseBody = await responseList.Content.ReadAsStringAsync();
            var allCompanies = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal(companyList, allCompanies);
            Assert.Equal(HttpStatusCode.OK, responseList.StatusCode);
        }

        [Fact]
        public async void Should_get_company_by_id_of_system_successfully()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var companyList = new List<Company>()
            {
                new Company(name: "SLB"),
                new Company(name: "TW"),
            };
            companyList[0].CompanyId = "SLB_ID";

            foreach (var company in companyList)
            {
                var companyJson = JsonConvert.SerializeObject(company);
                var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
                await httpClient.PostAsync("/companies", postBody);
            }

            //when
            var response = await httpClient.GetAsync($"companies/SLB_ID");

            // then
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var getCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal(companyList[0].Name, getCompany.Name);
        }
    }
}
