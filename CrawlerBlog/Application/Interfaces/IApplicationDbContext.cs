using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface IApplicationDbContext
    {
        public DbSet<Post> posts { get; set; }
        public DbSet<Comment> comments { get; set; }
    }
}
