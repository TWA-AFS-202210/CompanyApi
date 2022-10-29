using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class HelloControllerTest
    {
        [Fact]
        public async Task Should_return_hello_world_with_default_request()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();

            // when
            var response = await httpClient.GetAsync("/hello");
            var responseString = await response.Content.ReadAsStringAsync();

            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal("Hello World", responseString);
        }
    }
}
