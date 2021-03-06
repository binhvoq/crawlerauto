using Application.Contants;
using Application.Dto;
using Application.Interfaces;
using CrawlerAuto.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Handlers
{
    public class GetPostsHandler : IRequestHandler<GetPostsQuery, GetPostsResponnseDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<GetPostsHandler> _logger;
        public GetPostsHandler(IApplicationDbContext context, ILogger<GetPostsHandler> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<GetPostsResponnseDto> Handle(GetPostsQuery request, CancellationToken cancellationToken)
        {
            if (request.pageIndex <= 0 || request.pageSize <= 0)
            {
                var errorMessage = CustomErrorMessages.InvaildPageInput;
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }
            var pageIndex = request.pageIndex;
            int pageSize = request.pageSize;

            var skipItems = (pageIndex - 1) * pageSize;
            var result = _context.Posts.Include(c => c.comments).Select(x => new PostDto
            {
                id = x.id,
                uri = x.uri,
                title = x.title,
                summary = x.summary,
                totalComments = x.totalComments,
                comments = x.comments.Select(cmt => new CommentDto
                {
                    author = cmt.author,
                    content = cmt.content,
                    numberLike = cmt.numberLike
                }).OrderByDescending(o => o.numberLike).ToList()
            }).Skip(skipItems).Take(pageSize).ToList();


            return new GetPostsResponnseDto { 
                posts = result,
                totalPages = (_context.Posts.Count() + pageSize - 1) / pageSize
            };
        }
    }
}
