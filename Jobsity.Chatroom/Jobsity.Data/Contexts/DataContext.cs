using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Jobsity.Data.Contexts
{
    public class DbContext : DataContext
    {
        public DbContext(IConfiguration configuration) : base(configuration) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite(Configuration.GetConnectionString("JobsityDatabase"));
        }
    }
}
