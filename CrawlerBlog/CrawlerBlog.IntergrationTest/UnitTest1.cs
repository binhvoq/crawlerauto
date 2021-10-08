using Application.UseCases.Commands;
using Application.UseCases.Commands.Handler;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CrawlerBlog.IntergrationTest
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        public UnitTest1(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }
        [Fact]
        public async Task Test1Async()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var content = JsonConvert.SerializeObject(new {});

            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/Blog/addpost", stringContent);
            response.EnsureSuccessStatusCode();
            //response.StatusCode.Should().Be(HttpStatusCode.OK);
            var contents = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}
