using Microsoft.EntityFrameworkCore;
using TutoringSession.Models;

namespace TutoringSession.Data
{
    public class TutoringDbContext : DbContext
    {
        public TutoringDbContext(DbContextOptions<TutoringDbContext> options) : base(options) { }

        public DbSet<Session> Sessions => Set<Session>();

        /// <summary>
        /// Configure the EF Core model
        /// </summary>
        /// <param name="b"></param>
        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Session>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.SessionDate).HasColumnType("date");
                e.Property(x => x.HourlyRate).HasPrecision(10, 2);
                e.Property(x => x.FeeAmount).HasPrecision(10, 2);
            });
        }
    }
}
