using Application.Contants;
using Application.Interfaces;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Handlers;
using CrawlerAuto.Dto;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public class QueryTest
    {
        private readonly Mock<ILogger<GetPostsHandler>> _mockLogger;
        private readonly DbContextOptions<ApplicationDbContext> _options;
        public QueryTest()
        {
            _mockLogger = new Mock<ILogger<GetPostsHandler>>();
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                           .UseInMemoryDatabase(databaseName: "QueryTestDatabse")
                           .Options;
            Seed();       
        }
        [Fact]
        public async Task GetPostsQuery_ThrowException_WhenPageIndexNotVaild()
        {
            //Arrange
            GetPostsQuery input = new GetPostsQuery { pageIndex = 0, pageSize = 0 };

            //Act
            var mockObject = new Mock<IApplicationDbContext>();
            var getPostsHandler = new GetPostsHandler(mockObject.Object, _mockLogger.Object);
            Exception exception = await Assert.ThrowsAsync<Exception>(() => getPostsHandler.Handle(input, It.IsAny<CancellationToken>()));

            //Assert
            Assert.Equal(CustomErrorMessages.InvaildPageInput, exception.Message);
        }

        [Fact]
        public async Task GetPostsQuery_ReturnListPosts_WhenSuccess()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                //Arrange
                GetPostsQuery getPostsQuery = new GetPostsQuery { pageIndex = 1, pageSize = 2 };

                //Act
               var service = new GetPostsHandler(context, _mockLogger.Object);
               var result = await service.Handle(getPostsQuery, It.IsAny<CancellationToken>());
                
                //Assert
                Assert.NotNull(result);
            }        
        }

        private void Seed()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.Posts.Add(new Post { cmtId = "1", comments = new List<Comment>(), id = "1", summary = "dsf", title = "asdf", totalComments = "10", uri = "uri" });
                context.Posts.Add(new Post { cmtId = "2", comments = new List<Comment>(), id = "2", summary = "avef", title = "jrj", totalComments = "10", uri = "uri" });
                context.SaveChanges();
            }
        }

    }
}
