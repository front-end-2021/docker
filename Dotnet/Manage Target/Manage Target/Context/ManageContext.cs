using Manage_Target.Models;
using Microsoft.EntityFrameworkCore;

namespace Manage_Target.Context
{
    public class ManageContext : DbContext
    {
        public ManageContext(DbContextOptions<ManageContext> opts) : base(opts) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<Settings> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          //  base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Item>().ToTable("Item");
            modelBuilder.Entity<Models.Task>().ToTable("Task");
            modelBuilder.Entity<Settings>().ToTable("Settings");
        }
    }
}
