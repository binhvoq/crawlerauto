using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CrawlerBlog.IntergrationTest
{
    [CollectionDefinition("CrawlerBlogCollection")]
    public class CrawlerBlogCollectionDefinition :
        ICollectionFixture<CustomWebApplicationFactory<Startup>>
    {

    }
}
