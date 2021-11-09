using CrawlerAuto.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dto
{
    public class GetPostsResponnseDto
    {
        public IEnumerable<PostDto> posts { get; set; }
        public int totalPages { get; set; }
    }
}
