using System.Collections;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SpcBowling.Models
{
    public class SpcBowlingDbContext : DbContext
    {
        public SpcBowlingDbContext() : base("SpcBowlingDbContext") { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Score> Scores { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}