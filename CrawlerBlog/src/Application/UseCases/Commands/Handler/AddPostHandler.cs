using Application.Contants;
using Application.Interfaces;
using Application.Options;
using Domain.Entities;
using HtmlAgilityPack;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Commands.Handler
{
    public class AddPostHandler : IRequestHandler<AddPostsCommand, List<Post>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<AddPostHandler> _logger;
        private readonly WebHostDomainOption _webHostDomainOption;

        public AddPostHandler(IApplicationDbContext context, ILogger<AddPostHandler> logger, IOptions<WebHostDomainOption> webHostDomainOption) {
            _context = context;
            _logger = logger;
            _webHostDomainOption = webHostDomainOption.Value;
        }

        public async Task<List<Post>> Handle(AddPostsCommand request, CancellationToken cancellationToken)
        {
            var html = _webHostDomainOption.NguoiLaoDong;

            var nodes = LoadHtmlDoc(html).DocumentNode.SelectNodes("//div[@class='box-news-container']//div[@class='news-item ']");

            if (nodes == null) {
                var errorMessage = CustomErrorMessages.NodeReturnNull;
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            };

            List<Post> listBlogpost = new List<Post>();

            foreach (var node in nodes)
            {
                var blogTitle = node.SelectSingleNode(".//a[@class='title']")?.InnerHtml.Trim();
                var blogPostUri = (html + node.SelectSingleNode(".//a[@class='title']")?.Attributes["href"].Value).Trim();
                Post blogpost = new Post
                {
                    id = Guid.NewGuid().ToString(),
                    title = blogTitle,
                    uri = blogPostUri,
                    cmtId = blogPostUri.Split('-').ElementAt(blogPostUri.Split('-').Length - 1).Split('.')[0],
                    summary = LoadHtmlDoc(blogPostUri).DocumentNode.SelectSingleNode("//h2[@class='sapo-detail']")?.InnerText.Trim()
                };

                var isAnyNull = blogpost.title == null || blogpost.uri == null || blogpost.cmtId == null || blogpost.summary == null;
                if (isAnyNull || _context.Posts.Any(x => x.uri == blogPostUri)) continue; //not add when can't get data or already exist

                listBlogpost.Add(blogpost);
            }
            _context.Posts.AddRange(listBlogpost);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                var errorMessage = CustomErrorMessages.FailToSave;
                _logger.LogError(errorMessage);
                throw new DbUpdateException(errorMessage);
            }

            return listBlogpost;
        }

        private HtmlDocument LoadHtmlDoc(string html)
        {
            HtmlWeb web = new HtmlWeb();
            return web.Load(html);
        }
    }
}
