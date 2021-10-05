using Application.Interfaces;
using Application.Options;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Handler;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CrawlerBlog.UnitTest.UseCases
{
    public class MutationTest
    {
        [Fact]
        public async Task AddPostHandler_ReturnListPost_WhenSuccess()
        {
            var mockLogger = new Mock<ILogger<AddPostHandler>>();
            var mockOption = new Mock<IOptions<WebHostDomainOption>>();
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            mockOption.Setup(x => x.Value).Returns(new WebHostDomainOption { NguoiLaoDong = "https://nld.com.vn/" });
            // Insert seed data into the database using one instance of the context
            using (var context = new ApplicationDbContext(options))
            {
                context.Posts.Add(new Post { cmtId = "1", comments = new List<Comment>(), id = "1", summary = "dsf", title = "asdf", totalComments = "10", uri = "uri" });
                context.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var context = new ApplicationDbContext(options))
            {
                AddPostHandler services = new AddPostHandler(context, mockLogger.Object, mockOption.Object);
                List<Post> result = await services.Handle(new AddPostsCommand(), new CancellationToken());
                Assert.NotNull(result);
            }

        }

        //public async Task AddPostHandler_ThrowException_WhenFailToSavePosts()
        //{

        //}

        //public async Task AddPostHandler_ReturnListPosts_WhenSuccess()
        //{

        //}

        //public async Task AddCommentsCommand_ThrowException_WhenFailToSave()
        //{

        //}

        //public async Task AddCommentsCommand_ThrowException_WhenWebChangedTheirAPI()
        //{

        //}

        //public async Task AddCommentsCommand_ReturnChangeList_WhenSuccess()
        //{

        //}

    }
}
