using CrawlerAuto.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Queries
{
    public class GetPostsQuery: IRequest<IEnumerable<PostDto>>
    {
       public int pageIndex { get; set; }
    }
}
