using Application.UseCases.Queries;
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
    public class QueryTest
    {
        private readonly HttpClient _client;
        public QueryTest(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
        }

        [Fact]
        public async Task GetPostsQuery_ReturnListPosts_WhenSuccess()
        {
            // Arrange

            // Act
            var content = JsonConvert.SerializeObject(new GetPostsQuery
            {
                pageIndex = 1,
                pageSize = 5
            });

            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/Blog/getpost", stringContent);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var contents = await response.Content.ReadAsStringAsync();
            Assert.NotNull(contents);
        }
    }
}
