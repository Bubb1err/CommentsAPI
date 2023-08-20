using CommentsAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CommentsAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }
        public DbSet<Comment> Comments { get; set; }
    }
}
