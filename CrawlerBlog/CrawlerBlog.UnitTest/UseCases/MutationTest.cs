using Application.Contants;
using Application.Dto;
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
        private readonly DbContextOptions<ApplicationDbContext> _options;
        public MutationTest()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: "MutationTestDatabse")
                    .Options;
        }
        [Fact]
        public async Task AddPostHandler_ReturnListPost_WhenSuccess()
        {
            //Arrange
            Seed();
            var mockLogger = new Mock<ILogger<AddPostHandler>>();
            var mockOption = new Mock<IOptions<WebHostDomainOption>>();
            mockOption.Setup(x => x.Value).Returns(new WebHostDomainOption { NguoiLaoDong = "https://nld.com.vn/" });

            using (var context = new ApplicationDbContext(_options))
            {
                //Act
                AddPostHandler services = new AddPostHandler(context, mockLogger.Object, mockOption.Object);
                List<Post> result = await services.Handle(new AddPostsCommand(), new CancellationToken());

                //Assert
                Assert.NotNull(result);
            }
        }

        [Fact]
        public async Task AddPostHandler_ThrowException_WhenWrongDomain()
        {
            //Arrange
            Seed();
            var mockLogger = new Mock<ILogger<AddPostHandler>>();
            var mockOption = new Mock<IOptions<WebHostDomainOption>>();
            mockOption.Setup(x => x.Value).Returns(new WebHostDomainOption { NguoiLaoDong = "https://nd.com.vn/" });

            using (var context = new ApplicationDbContext(_options))
            {
                //Act
                AddPostHandler services = new AddPostHandler(context, mockLogger.Object, mockOption.Object);

                //Assert
                var exception = await Assert.ThrowsAsync<Exception>(() => services.Handle(new AddPostsCommand(), new CancellationToken()));
                Assert.Equal(CustomErrorMessages.WrongDomain, exception.Message);
            }
        }

        [Fact]
        public async Task AddCommentsCommand_ReturnChangeList_WhenSuccess()
        {
            //Arrange
            Seed();
            var mockLogger = new Mock<ILogger<AddCommentsHandler>>();

            using (var context = new ApplicationDbContext(_options))
            {
                //Act
                AddCommentsHandler services = new AddCommentsHandler(context, mockLogger.Object);
                List<ChangeListDto> result = await services.Handle(new AddCommentsCommand(), new CancellationToken());

                //Assert
                Assert.NotNull(result);
            }
        }

        //public async Task AddCommentsCommand_ThrowException_WhenWebChangedTheirAPI()

        private void Seed()
        {
            using (var context = new ApplicationDbContext(_options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                context.Posts.Add(new Post { cmtId = "1", comments = new List<Comment>(), id = "20210923143843924", summary = "dsf", title = "asdf", totalComments = "10", uri = "uri" });
                context.SaveChanges();
            }
        }
    }
}
