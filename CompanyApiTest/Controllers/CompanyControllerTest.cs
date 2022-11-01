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
        public async void Should_return_conflict_when_creat_same_Companies()
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
    }
}
