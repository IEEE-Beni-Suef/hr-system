using IEEE.Configurations;
using IEEE.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IEEE.Data
{
    public class AppDbContext : IdentityDbContext<User, ApplicationRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TaskConfigurations());
            modelBuilder.ApplyConfiguration(new Users_TasksConfigurations());
            modelBuilder.ApplyConfiguration(new MeetingConfig());
            modelBuilder.ApplyConfiguration(new CommitteeConfig());
            modelBuilder.ApplyConfiguration(new Users_MeetingsCong());


            modelBuilder.Entity<User>()
           .HasOne(u => u.Role)
           .WithMany(r => r.Users)
           .HasForeignKey(u => u.RoleId)
           .HasPrincipalKey(r => r.Id) 
           .OnDelete(DeleteBehavior.Restrict);



            modelBuilder
                .Entity<User>()
                .Property(u => u.Goverment)
                .HasConversion<string>();

            modelBuilder
                .Entity<User>()
                .Property(u => u.Year)
                .HasConversion<string>();

            modelBuilder
                .Entity<User>()
                .Property(u => u.Sex)
                .HasConversion<string>();

            modelBuilder.Entity<User>()
                .Property(u => u.Faculty)
                .HasConversion<string>();

            // علاقة الـ Vices (واحد -> متعدد)
            modelBuilder.Entity<Committee>()
                .HasMany(c => c.Vices)
                .WithOne(u => u.ViceCommittee)
                .HasForeignKey(u => u.CommitteeId)
                .OnDelete(DeleteBehavior.Restrict);




        }

        // public DbSet<User> Users { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Users_Tasks> Users_Tasks { get; set; }
        public DbSet<Users_Meetings> Users_Meetings { get; set; }
        public DbSet<Committee> Committees { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
    }
}
