using Microsoft.EntityFrameworkCore;

namespace ASM_SIMS.DB
{
    public class SimsDataContext : DbContext
    {
        public SimsDataContext(DbContextOptions<SimsDataContext> options) : base(options)
        {
        }

        //Truyen vao DbSet de mapping voi bang trong database
        // gen ra cac bang trong database
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Courses> Courses { get; set; }
        public DbSet<Account> Account { get; set; }
    }
}
