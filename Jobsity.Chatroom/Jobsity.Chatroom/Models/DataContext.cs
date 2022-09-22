using Microsoft.EntityFrameworkCore;

namespace Jobsity.Chatroom.Models
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(Configuration.GetConnectionString("JobsityDatabase"));
        }

        public DbSet<Message> Messages { get; set; }
    }
}
