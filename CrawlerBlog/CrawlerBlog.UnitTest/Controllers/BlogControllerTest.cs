using Application.Interfaces;
using Application.UseCases.Queries;
using CrawlerAuto.Dto;
using CrawlerBlog.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CrawlerBlog.UnitTest.Controllers
{
    public class BlogControllerTest
    {
        private readonly Mock<ISender> mediatorMock;
        private readonly Mock<IApplicationDbContext> contextMock;
        private readonly BlogController controller;

        public BlogControllerTest()
        {
            mediatorMock = new Mock<ISender>();
            contextMock = new Mock<IApplicationDbContext>();
            controller = new BlogController(contextMock.Object, mediatorMock.Object);
        }
        [Fact]
        public async Task GetBlogPost_ReturnBlogPost_WhenSuccess()
        {
            //Arrange
            mediatorMock.Setup(x => x.Send(new List<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<PostDto>());
            
            //Act
            var result = controller.GetBlogPost(new GetPostsQuery {pageIndex = 1, pageSize = 1}).Result.Result;

            //Assert
            ObjectResult objectResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
        }
    }
}
