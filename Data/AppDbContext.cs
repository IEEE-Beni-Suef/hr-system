using IEEE.Configurations;
using IEEE.Entities;
using Microsoft.EntityFrameworkCore;

namespace IEEE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserConfigurations());
            modelBuilder.ApplyConfiguration(new TaskConfigurations());

            modelBuilder.ApplyConfiguration(new Users_TasksConfigurations());


        }


        public DbSet<User> Users { get; set; }

        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Tasks> Users_Tasks { get; set; }

        public DbSet<Role> Roles { get; set; }





    }
}
