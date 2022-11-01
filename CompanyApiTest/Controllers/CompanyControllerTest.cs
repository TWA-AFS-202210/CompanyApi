using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using CompanyApi.Model;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async void Should_create_Company_Successfully()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company(name: "SLB");
            var serializedObject = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            //when
            var response = await httpClient.PostAsync("companies", postBody);
            //then
            //response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdcompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal("SLB", createdcompany.Name);
            Assert.NotEmpty(createdcompany.ID);
        }

        [Fact]
        public async void Should_return_conflict_when_create_same_Companies()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            var company = new Company(name: "SLB");
            var serializedObject = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("companies", postBody);
            var response = await httpClient.PostAsync("companies", postBody);
            //response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async void Should_get_all_companies()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("companies");
            var company = new Company(name: "Google");
            var serializedObject = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("companies", postBody);
            //when
            var response = await httpClient.GetAsync("companies");
            //then
            //response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var allcompanies = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal(company.Name, allcompanies[0].Name);
        }

        [Fact]
        public async void Should_get_existing_companies()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("companies");
            var company = new Company(name: "IBM");
            var serializedObject = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdcompany = JsonConvert.DeserializeObject<Company>(responseBody);
            //when
            var newresponse = await httpClient.GetAsync($"companies/{createdcompany.ID}");
            var newresponseBody = await newresponse.Content.ReadAsStringAsync();
            var existingcompany = JsonConvert.DeserializeObject<Company>(newresponseBody);
            //then
            //response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, newresponse.StatusCode);
            Assert.Equal(createdcompany.ID, existingcompany.ID);
        }
    }
}
