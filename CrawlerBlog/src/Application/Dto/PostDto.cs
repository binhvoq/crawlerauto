using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrawlerAuto.Dto
{
    public class PostDto
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string title { get; set; }
        public string summary { get; set; }
        public string totalComments { get; set; }
        public List<CommentDto> comments { get; set; }
    }
}
