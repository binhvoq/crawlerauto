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
            var listPostUpdate = _context.Posts.ToList();
            if (listPostUpdate.Count <= 0)
            {
                var errorMessage = "Cannot get blog post list";
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            var changeList = new List<ChangeListDto>();

            foreach (var post in listPostUpdate)
            {
                var listComments = await getNguoiLaoDongCommentAsync(post.cmtId);
                UpdateChangeList(changeList, post, listComments.Count.ToString()); //do not change order

                post.totalComments = listComments.Count.ToString();
                post.comments = (listComments.Count > 0) ? listComments : null;
                _context.Entry(post).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                var errorMessage = "Error when save Db";
                _logger.LogError(errorMessage);
                throw new DbUpdateException(errorMessage);
            }

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
                throw new Exception("API changed or wrong cmtId");
            }
            catch (Exception)
            {
                throw new Exception("Error when get comment");
            }
        }

        private void UpdateChangeList(List<ChangeListDto> changeList, Post post, string newTotalComments)
        {
            if (newTotalComments != post.totalComments && newTotalComments != "0") changeList.Add(new ChangeListDto
            {
                id = post.id,
                title = post.title,
                uri = post.uri,
                commentsUpdated = (post.totalComments ?? "0") + " -> " + newTotalComments
            });
        }
    }
}
