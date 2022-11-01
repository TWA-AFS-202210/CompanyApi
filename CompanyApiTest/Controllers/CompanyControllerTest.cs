using System.Collections;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Xunit.Sdk;
using System.Reflection;

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
            var company = new Company(name: "SLB");

            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var addCompany = await httpClient.PostAsync("/companies", postBody);
            var addCompanyBody = await addCompany.Content.ReadAsStringAsync();
            var companyId = JsonConvert.DeserializeObject<Company>(addCompanyBody).CompanyId;

            //when
            var response = await httpClient.GetAsync($"companies/{companyId}");

            // then
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var getCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal(company.Name, getCompany.Name);
        }

        [Fact]
        public async void Should_return_not_found_when_get_no_exist_company_by_id()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");

            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies", postBody);

            //when
            var response = await httpClient.GetAsync("companies/OtherCompany_ID");

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Should_obtain_X_page_size_companies_from_index_of_Y()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var companyList = new List<Company>()
            {
                new Company(name: "SLB"),
                new Company(name: "TW"),
                new Company(name: "APPLE"),
                new Company(name: "MICROSOFT"),
            };

            foreach (var company in companyList)
            {
                var companyJson = JsonConvert.SerializeObject(company);
                var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
                await httpClient.PostAsync("/companies", postBody);
            }

            //when
            var responseList = await httpClient.GetAsync("/companies?pageSize=2&&pageIndex=2");

            // then
            var getCompanies = new List<Company>()
            {
                new Company(name: "APPLE"),
                new Company(name: "MICROSOFT"),
            };
            responseList.EnsureSuccessStatusCode();
            var responseBody = await responseList.Content.ReadAsStringAsync();
            var allCompanies = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal(getCompanies, allCompanies);
            Assert.Equal(HttpStatusCode.OK, responseList.StatusCode);
        }

        [Fact]
        public async void Should_update_basic_information_of_an_existing_company()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var originCompany = JsonConvert.DeserializeObject<Company>(responseBody);

            company.Name = "slb";
            var modifyCompanyJson = JsonConvert.SerializeObject(company);
            var modifyPostBody = new StringContent(modifyCompanyJson, Encoding.UTF8, "application/json");

            //when
            var companyId = originCompany.CompanyId;
            var modifyResponse = await httpClient.PutAsync($"/companies/{companyId}", modifyPostBody);

            // then
            modifyResponse.EnsureSuccessStatusCode();
            var resultResponseBody = await modifyResponse.Content.ReadAsStringAsync();
            var resultCompany = JsonConvert.DeserializeObject<Company>(resultResponseBody);
            Assert.Equal(company, resultCompany);
            Assert.Equal(HttpStatusCode.OK, modifyResponse.StatusCode);
        }

        [Fact]
        public async void Should_return_not_found_of_not_existing_company_when_modify()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/companies", postBody);

            //when
            var companyId = "otherCompanyId";
            var modifyResponse = await httpClient.PutAsync($"/companies/{companyId}", postBody);

            // then
            Assert.Equal(HttpStatusCode.NotFound, modifyResponse.StatusCode);
        }
    }
}
