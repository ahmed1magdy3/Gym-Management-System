using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymManagementSystem.Context
{
    public class GymDbContext : DbContext
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("server=localhost\\MSSQLSERVER01;Database=GymDb;trusted_Connection=true;trustServerCertificate=true;");
        //}
        public GymDbContext(DbContextOptions<GymDbContext>options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Plan> Plans { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<Member> Member { get; set; }
        public DbSet<HealthRecord> HealthRecord { get; set; }
        public DbSet<Trainer> Trainer { get; set; }
        public DbSet<MemberShip> MemberShip { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<Category> Category { get; set; }


    }
}
