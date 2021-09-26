using Application.UseCases.Queries;
using CrawlerAuto.Dto;
using Google.Protobuf.WellKnownTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CrawlerBlog.Controllers
{
    public class BlogControllerTest
    {
        [Fact]
        public async Task GetBlogPost_ReturnBlogPost_WhenSuccess()
        {
            //Arrange
            var mediatorMock = new Mock<ISender>();
            mediatorMock.Setup(x => x.Send(It.IsAny<GetPostsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<IEnumerable<PostDto>>());

            //Act
        }
    }
}
