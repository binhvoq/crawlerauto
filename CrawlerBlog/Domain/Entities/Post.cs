using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("Post")]
    public class Post
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string title { get; set; }
        public string cmtId { get; set; }
        public string summary { get; set; }
        public string totalComments { get; set; }
        public List<Comment> comments { get; set; }
    }
}
