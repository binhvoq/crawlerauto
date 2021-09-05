using Application.Dto;
using CrawlerAuto.Dto;
using Domain.Entities;
using HtmlAgilityPack;
using Infrastructure.Data;
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
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{pageIndex}")]
        public async Task<ActionResult<IEnumerable<PostDto>>> GetBlogPost(int pageIndex)
        {
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

        [HttpPost("getpost")]
        public async Task<ActionResult<List<Post>>> AddBlogPost()
        {

            var html = @"https://nld.com.vn/";

            var nodes = LoadHtmlDoc(html).DocumentNode.SelectNodes("//div[@class='box-news-container']//div[@class='news-item ']");

            if (nodes == null) throw new Exception("Classes changed or can not get html");

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
                throw;
            }

            return Ok(listBlogpost);
        }

        [HttpPost("getcmts")]
        public async Task<ActionResult<List<ChangeListDto>>> GetComments()
        {
            var listPostUpdate = _context.Posts.ToList();
            if (listPostUpdate.Count <= 0) throw new Exception("Cannot get blog post list");

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
                throw new DbUpdateException("Error when save Db");
            }

            if (changeList.Count == 0)
            {
                return Ok("No comments are update");
            }
         
            return Ok(changeList);
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
            catch (HttpRequestException exc) {
                throw new Exception("API changed or wrong cmtId");
            }
            catch (Exception exc)
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
