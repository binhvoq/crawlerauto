using Application.Interfaces;
using Application.Options;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Handler;
using Domain.Entities;
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
        public async Task AddPostHandler_ThrowException_WhenNodeReturnNull()
        {
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<AddPostHandler>>();
            var mockOption = new Mock<IOptions<WebHostDomainOption>>();

            var dbSet = new Mock<DbSet<Post>>();
            dbSet.Setup(x => x.Any(x => x.uri == It.IsAny<string>())).Returns(false);


            mockContext.Setup(x => x.Posts).Returns(dbSet.Object);

            mockOption.Setup(x => x.Value).Returns(new WebHostDomainOption { NguoiLaoDong = "https://nld.com.vn/" });

            var service = new AddPostHandler(mockContext.Object, mockLogger.Object, mockOption.Object);

            //Act
            var result = service.Handle(new AddPostsCommand(), It.IsAny<CancellationToken>());

            var resul6t = result;

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
