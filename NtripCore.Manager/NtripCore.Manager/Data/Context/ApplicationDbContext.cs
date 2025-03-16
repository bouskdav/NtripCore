using Microsoft.EntityFrameworkCore;

namespace NtripCore.Manager.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationSettings> Settings { get; set; }
    }
}
