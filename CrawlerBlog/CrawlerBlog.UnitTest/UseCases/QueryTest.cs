using Application.Contants;
using Application.Interfaces;
using Application.UseCases.Queries;
using Application.UseCases.Queries.Handlers;
using CrawlerAuto.Dto;
using Domain.Entities;
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
        private readonly Mock<IApplicationDbContext> _mockContext;
        private readonly Mock<ILogger<GetPostsHandler>> _mockLogger;
        private readonly GetPostsHandler getPostsHandler;
        public QueryTest()
        {
            _mockContext = new Mock<IApplicationDbContext>();
            _mockLogger = new Mock<ILogger<GetPostsHandler>>();
            getPostsHandler = new GetPostsHandler(_mockContext.Object, _mockLogger.Object);
        }
        [Fact]
        public async Task GetPostsQuery_ThrowException_WhenPageIndexNotVaild()
        {
            //Arrange
            GetPostsQuery input = new GetPostsQuery { pageIndex = 0, pageSize = 0 };

            //Act
            Exception exception = await Assert.ThrowsAsync<Exception>(() => getPostsHandler.Handle(input, It.IsAny<CancellationToken>()));

            //Assert
            Assert.Equal(CustomErrorMessages.InvaildPageInput, exception.Message);
        }

        [Fact]
        public async Task GetPostsQuery_ReturnListPosts_WhenSuccess()
        {
            //Arrange
            var data = new List<Post>
            {
                new Post {
                comments = new List<Comment>(),
                id = "001",
                summary = "sum",
                title = "title",
                uri = "url"
                },
                new Post {
                comments = new List<Comment>(),
                id = "001",
                summary = "sum",
                title = "title",
                uri = "url"
                },
                new Post {
                comments = new List<Comment>(),
                id = "001",
                summary = "sum",
                title = "title",
                uri = "url"
                },
            };
            var mockContext = new Mock<IApplicationDbContext>();
            mockContext.Setup(c => c.Posts).Returns(GetQueryableMockDbSet(data));
            var service = new GetPostsHandler(mockContext.Object, new Mock<ILogger<GetPostsHandler>>().Object);

            //Act
            var result = await service.Handle(new GetPostsQuery {pageIndex = 1, pageSize = 1 },It.IsAny<CancellationToken>());

            //Assert
            Assert.NotNull(result);

        }

        private static DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return dbSet.Object;
        }

    }
}
