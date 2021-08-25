using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("Comment")]
    public class Comment
    {
        public string id { get; set; }
        public string author { get; set; }
        public string content { get; set; }
        public string numberLike { get; set; }
        public Post post { get; set; }
    }
}
