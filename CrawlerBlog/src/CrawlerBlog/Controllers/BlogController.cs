using Application.Contants;
using Application.Dto;
using Application.Interfaces;
using Application.UseCases.Commands;
using Application.UseCases.Queries;
using CrawlerAuto.Dto;
using Domain.Entities;
using HtmlAgilityPack;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CrawlerBlog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IApplicationDbContext _context;
        private readonly ISender _mediator;

        public BlogController(IApplicationDbContext context, ISender mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        [HttpPost("getpost")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetBlogPost(GetPostsQuery getPostsQuery)
        {
            var result = await _mediator.Send(getPostsQuery);
            return Ok(result);
        }

        [HttpPost("addpost")]
        public async Task<ActionResult<List<Post>>> AddBlogPost()
        {
            var result = await _mediator.Send(new AddPostsCommand());

            if (result.Count == 0) return Ok(CustomErrorMessages.NoPostToUpdate);
            return Ok(result);
        }

        [HttpPost("addcmts")]
        public async Task<ActionResult<List<ChangeListDto>>> AddComments()
        {
            var result = await _mediator.Send(new AddCommentsCommand());

            if (result.Count == 0) return Ok("No comments are update");
            return Ok(result);
        }

        // PUT: api/BlogPosts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogPost(string id, Post blogPost)
        {
            if (id != blogPost.id)
            {
                return BadRequest();
            }

            _context.Entry(blogPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Post>> DeleteBlogPost(string id)
        {
            var blogPost = await _context.Posts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(blogPost);
            await _context.SaveChangesAsync();

            return blogPost;
        }

        private bool BlogPostExists(string id)
        {
            return _context.Posts.Any(e => e.id == id);
        }

        private HtmlDocument LoadHtmlDoc(string html)
        {
            HtmlWeb web = new HtmlWeb();
            return web.Load(html);
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
            catch (HttpRequestException) {
                throw new Exception("API changed or wrong cmtId");
            }
            catch (Exception)
            {
                throw new Exception("Error when get comment");
            }
        }

        private void UpdateChangeList(List<ChangeListDto> changeList, Post post, string newTotalComments) {
            if (newTotalComments != post.totalComments && newTotalComments != "0") changeList.Add(new ChangeListDto
            {
                id = post.id,
                title = post.title,
                commentsUpdated = (post.totalComments ?? "0") + " -> " + newTotalComments
            });
        }
    }
}
