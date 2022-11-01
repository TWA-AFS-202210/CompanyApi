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

            var postBody = SerializeCompanyToStringContent(company);

            // when
            var response = await httpClient.PostAsync("/companies", postBody);

            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdCompany = await DeserializeObjectToCompany(response);
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotNull(createdCompany.CompanyId);
        }

        [Fact]
        public async void Should_return_409_when_add_exist_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");

            var postBody = SerializeCompanyToStringContent(company);
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
                var postBody = SerializeCompanyToStringContent(company);
                await httpClient.PostAsync("/companies", postBody);
            }

            //when
            var responseList = await httpClient.GetAsync("/companies");

            // then
            responseList.EnsureSuccessStatusCode();
            var allCompanies = DeserializeObjectToCompanyList(responseList).Result;
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

            var postBody = SerializeCompanyToStringContent(company);
            var addCompany = await httpClient.PostAsync("/companies", postBody);

            var companyId = DeserializeObjectToCompany(addCompany).Result.CompanyId;

            //when
            var response = await httpClient.GetAsync($"companies/{companyId}");

            // then
            response.EnsureSuccessStatusCode();
            var getCompany = DeserializeObjectToCompany(response).Result;
            Assert.Equal(company.Name, getCompany.Name);
        }

        [Fact]
        public async void Should_return_not_found_when_get_no_exist_company_by_id()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");

            var postBody = SerializeCompanyToStringContent(company);
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
                var postBody = SerializeCompanyToStringContent(company);
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
            var allCompanies = DeserializeObjectToCompanyList(responseList).Result;
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
            var postBody = SerializeCompanyToStringContent(company);
            var response = await httpClient.PostAsync("/companies", postBody);
            var originCompany = DeserializeObjectToCompany(response).Result;

            company.Name = "slb";
            var modifyPostBody = SerializeCompanyToStringContent(company);

            //when
            var companyId = originCompany.CompanyId;
            var modifyResponse = await httpClient.PutAsync($"/companies/{companyId}", modifyPostBody);

            // then
            modifyResponse.EnsureSuccessStatusCode();
            var resultCompany = DeserializeObjectToCompany(modifyResponse).Result;
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
            var postBody = SerializeCompanyToStringContent(company);
            await httpClient.PostAsync("/companies", postBody);

            //when
            var companyId = "otherCompanyId";
            var modifyResponse = await httpClient.PutAsync($"/companies/{companyId}", postBody);

            // then
            Assert.Equal(HttpStatusCode.NotFound, modifyResponse.StatusCode);
        }

        [Fact]
        public async void Add_an_employee_to_a_specific_company()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company(name: "SLB");
            var response = await httpClient.PostAsync("/companies", SerializeCompanyToStringContent(company));
            var companyId = DeserializeObjectToCompany(response).Result.CompanyId;

            var employee = new List<Employee>() { new Employee(name: "YZJ", salary: 10), };
            var employeePostBody = SerializeEmployeeListToStringContent(employee);

            //when
            var addEmployeeResponse = await httpClient.PostAsync($"/companies/{companyId}", employeePostBody);

            // then
            addEmployeeResponse.EnsureSuccessStatusCode();
            var addEmployee = DeserializeObjectToEmployeeList(addEmployeeResponse).Result;
            Assert.Equal(HttpStatusCode.OK, addEmployeeResponse.StatusCode);
            Assert.Equal(employee, addEmployee);
        }

        [Fact]
        public async void Obtain_employee_list_of_specific_company()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company(name: "SLB");
            var response = await httpClient.PostAsync("/companies", SerializeCompanyToStringContent(company));
            var companyId = DeserializeObjectToCompany(response).Result.CompanyId;

            var employeeList = new List<Employee>()
            {
                new Employee(name: "YZJ", salary: 10),
                new Employee(name: "LJ", salary: 11),
                new Employee(name: "LWR", salary: 12),
            };
            var employeePostBody = SerializeEmployeeListToStringContent(employeeList);
            await httpClient.PostAsync($"/companies/{companyId}", employeePostBody);

            //when
            var addEmployeeListResponse = await httpClient.GetAsync($"/companies/{companyId}/employees");

            // then
            addEmployeeListResponse.EnsureSuccessStatusCode();

            var addEmployeeList = DeserializeObjectToEmployeeList(addEmployeeListResponse).Result;
            Assert.Equal(HttpStatusCode.OK, addEmployeeListResponse.StatusCode);
            Assert.Equal(employeeList, addEmployeeList);
        }

        private static StringContent SerializeCompanyToStringContent(Company company)
        {
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");
            return postBody;
        }

        private static async Task<Company> DeserializeObjectToCompany(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var company = JsonConvert.DeserializeObject<Company>(responseBody);
            return company;
        }

        private static async Task<List<Company>> DeserializeObjectToCompanyList(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var company = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            return company;
        }

        private static StringContent SerializeEmployeeToStringContent(Employee employee)
        {
            var employeeJson = JsonConvert.SerializeObject(employee);
            var postBody = new StringContent(employeeJson, Encoding.UTF8, "application/json");
            return postBody;
        }

        private static StringContent SerializeEmployeeListToStringContent(List<Employee> employee)
        {
            var employeeJson = JsonConvert.SerializeObject(employee);
            var postBody = new StringContent(employeeJson, Encoding.UTF8, "application/json");
            return postBody;
        }

        private static async Task<Employee> DeserializeObjectToEmployee(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var employee = JsonConvert.DeserializeObject<Employee>(responseBody);
            return employee;
        }

        private static async Task<List<Employee>> DeserializeObjectToEmployeeList(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var employeeList = JsonConvert.DeserializeObject<List<Employee>>(responseBody);
            return employeeList;
        }
    }
}
