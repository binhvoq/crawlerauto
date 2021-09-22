using Application.Interfaces;
using CrawlerAuto.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Queries.Handlers
{
    public class GetPostsHandler : IRequestHandler<GetPostsQuery, IEnumerable<PostDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetPostsHandler(IApplicationDbContext context) {
            _context = context;
        }
        public async Task<IEnumerable<PostDto>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
        {
            var pageIndex = request.pageIndex;

            int pageSize = 8;
            var skipItems = (pageIndex - 1) * pageSize;
            var result = await _context.Posts.Include(c => c.comments).Select(x => new PostDto
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
                }).ToList()
            }).Skip(skipItems).Take(pageSize).ToListAsync();

            return result;
        }
    }
}
