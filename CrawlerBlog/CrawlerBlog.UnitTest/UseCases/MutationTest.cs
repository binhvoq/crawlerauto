using Application.Interfaces;
using Application.UseCases.Commands;
using Application.UseCases.Commands.Handler;
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
    public class MutationTest
    {
        [Fact]
        public async Task AddPostHandler_ThrowException_WhenNodeReturnNull()
        {
            var mockContext = new Mock<IApplicationDbContext>();
            var mockLogger = new Mock<ILogger<AddPostHandler>>();
            var service = new AddPostHandler(mockContext.Object, mockLogger.Object);

            //Act
            var result = service.Handle(new AddPostsCommand(), It.IsAny<CancellationToken>());

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
