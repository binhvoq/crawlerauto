using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CrawlerBlog.IntergrationTest.UseCases
{
    [Collection("CrawlerBlogCollection")]
    public class MutationTest
    {
        private readonly HttpClient _client;
        public MutationTest(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        }

        [Fact]
        public async Task AddPostHandler_ReturnListPost_WhenSuccess()
        {
            // Arrange

            // Act
            var content = JsonConvert.SerializeObject(new { });

            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/Blog/addpost", stringContent);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var contents = await response.Content.ReadAsStringAsync();
            Assert.NotNull(contents);
        }

        [Fact]
        public async Task AddCommentsCommand_ReturnChangeList_WhenSuccess()
        {
            // Arrange

            // Act
            var content = JsonConvert.SerializeObject(new { });

            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/Blog/addcmts", stringContent);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var contents = await response.Content.ReadAsStringAsync();
            Assert.NotNull(contents);
        }
    }
}
