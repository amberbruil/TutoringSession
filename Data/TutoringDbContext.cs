using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using TutoringSession.Models;

namespace TutoringSession.Data
{
    public class TutoringDbContext : DbContext
    {
        public TutoringDbContext(DbContextOptions<TutoringDbContext> options) : base(options) { }

        public DbSet<Session> Sessions => Set<Session>();

        public DbSet<User> Users => Set<User>();

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

            b.Entity<User>().HasData(
               new User { Id = "T1001", PasswordHash = PasswordHasher.Hash("tutorpass"), Role = "Tutor" },
               new User { Id = "S2001", PasswordHash = PasswordHasher.Hash("studentpass"), Role = "Student" }
            );
        }

        /// <summary>
        /// Simple SHA256 password hasher
        /// </summary>
        public static class PasswordHasher
        {
            public static string Hash(string input)
            {
                using var sha256 = System.Security.Cryptography.SHA256.Create();
                var bytes = System.Text.Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToHexString(hash);
            }
        }
    }
}
