using Application.Contants;
using Application.Dto;
using Application.Interfaces;
using Domain.Entities;
using HtmlAgilityPack;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Commands.Handler
{
    public class AddCommentsHandler : IRequestHandler<AddCommentsCommand, List<ChangeListDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<AddCommentsHandler> _logger;
        public AddCommentsHandler(IApplicationDbContext context, ILogger<AddCommentsHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<ChangeListDto>> Handle(AddCommentsCommand request, CancellationToken cancellationToken)
        {
            var changeList = new List<ChangeListDto>();

            var listPostUpdate = _context.Posts.Include(x => x.comments).ToList();
            if (listPostUpdate.Count <= 0) return changeList;

            foreach (var post in listPostUpdate)
            {
                var listComments = await getNguoiLaoDongCommentAsync(post.cmtId);
                UpdateChangeList(changeList, post, listComments.Count.ToString()); //do not change order

                var newComments = listComments.FindAll(x => !post.comments.Select(y => y.content).Contains(x.content)
                && !post.comments.Select(y => y.author).Contains(x.author));  //filter new comments

                //Update numberLike
                post.comments = post.comments.Select(x => {
                    x.numberLike = listComments.Any(y => y.author == x.author && y.content == x.content) ? listComments.Find(y => y.author == x.author && y.content == x.content).numberLike : x.numberLike;
                    return x;
                }).ToList();

                //Add new comments
                if (newComments.Count == 0) continue;
                post.comments.AddRange(newComments.Select(x => { x.post = post; return x; }));
                post.totalComments = listComments.Count.ToString();

                _context.Entry(post).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return changeList;
        }

        private async Task<List<Comment>> getNguoiLaoDongCommentAsync(string cmtId)
        {
            List<Comment> listComments = new List<Comment>();
            using var client = new HttpClient();
            try
            {
                var html = await client.GetStringAsync($"https://comment.nld.com.vn/ajax/ListComment-n{cmtId}-i1-s5-o1.htm");
                if (String.IsNullOrEmpty(html)) return listComments;

                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                var nodes = doc.DocumentNode.SelectNodes("//div[@class='comment-detail']");
                foreach (var node in nodes)
                {
                    Comment cmt = new Comment
                    {
                        id = Guid.NewGuid().ToString(),
                        author = node.SelectSingleNode(".//div[@class='username']")?.InnerHtml.Trim(),
                        content = node.SelectSingleNode(".//div[@class='comment-description']/p | .//div[@class='comment-description']")?.InnerHtml.Replace("<p>", "").Replace("</p>", "").Trim(),
                        numberLike = node.SelectSingleNode(".//span[contains(@class,'thich')]")?.InnerHtml.Trim()
                    };
                    var isAnyNull = cmt.author == null || cmt.content == null || cmt.numberLike == null;
                    if (isAnyNull) continue;
                    listComments.Add(cmt);
                }

                return listComments;
            }
            catch (HttpRequestException)
            {
                throw new Exception(CustomErrorMessages.CmtAPIChanged);
            }
        }

        private void UpdateChangeList(List<ChangeListDto> changeList, Post post, string newTotalComments)
        {
            if (newTotalComments != post.comments.Count.ToString() && newTotalComments != "0") changeList.Add(new ChangeListDto
            {
                id = post.id,
                title = post.title,
                uri = post.uri,
                commentsUpdated = (post.comments.Count.ToString() ?? "0") + " -> " + newTotalComments
            });
        }
    }
}
