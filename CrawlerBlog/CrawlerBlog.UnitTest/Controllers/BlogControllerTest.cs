using Application.Contants;
using Application.Interfaces;
using Application.UseCases.Commands;
using Application.UseCases.Queries;
using CrawlerAuto.Dto;
using CrawlerBlog.Controllers;
using Domain.Entities;
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
        public void GetBlogPost_ReturnBlogPosts_WhenSuccess()
        {
            //Arrange
            mediatorMock.Setup(x => x.Send(It.IsAny<GetPostsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(Enumerable.Empty<PostDto>());

            //Act
            var result = controller.GetBlogPost(new GetPostsQuery { pageIndex = 1, pageSize = 1 }).Result.Result;

            //Assert
            ObjectResult objectResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);        }

        [Fact]
        public void AddBlogPost_ReturnBlogPosts_WhenSuccess()
        {
            //Arrange
            var responseList = new List<Post>();
            responseList.Add(new Post
            {
                cmtId = "1",
                comments = new List<Comment>(),
                id = "1",
                summary = "abc",
                title = "Title",
                totalComments = "10",
                uri = "url"

            });
            mediatorMock.Setup(x => x.Send(It.IsAny<AddPostsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(responseList);

            //Act
            var result = controller.AddBlogPost().Result.Result;

            //Assert
            ObjectResult objectResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
            Assert.IsType<List<Post>>(objectResult.Value);
        }

        [Fact]
        public void AddBlogPost_ReturnInformationMessage_WhenNoPostToUpdate()
        {
            //Arrange
            mediatorMock.Setup(x => x.Send(It.IsAny<AddPostsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<Post>());

            //Act
            var result = controller.AddBlogPost().Result.Result;

            //Assert
            ObjectResult objectResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
            Assert.Equal(CustomErrorMessages.NoPostToUpdate, objectResult.Value);
        }
    }
}
