using Microsoft.EntityFrameworkCore;

namespace ASM_SIMS.DB
{
    public class SimsDataContext : DbContext
    {
        public SimsDataContext(DbContextOptions<SimsDataContext> options) : base(options)
        {
        }

        public DbSet<Categories> Categories { get; set; }
    }
}
