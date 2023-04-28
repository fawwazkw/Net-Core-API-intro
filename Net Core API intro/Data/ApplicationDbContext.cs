using Microsoft.EntityFrameworkCore;
using Net_Core_API_intro.Models;
using System.Collections.Generic;

namespace Net_Core_API_intro.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
    }
}
