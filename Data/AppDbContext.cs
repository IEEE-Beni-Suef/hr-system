using IEEE.Configurations;
using IEEE.Entities;
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
            modelBuilder.ApplyConfiguration(new EventConfig());
            modelBuilder.ApplyConfiguration(new EventCategoryConfig());





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

            modelBuilder.Entity<Committee>()
      .HasMany(c => c.Vices)
      .WithOne(u => u.ViceCommittee)
      .HasForeignKey(u => u.ViceCommitteeId)
      .OnDelete(DeleteBehavior.Restrict); // أو Cascade حسب ما تحب





            // Configure Category-Article relationship
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Articles)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Article-Subsection relationship
            modelBuilder.Entity<Subsection>()
                .HasOne(s => s.Article)
                .WithMany(a => a.Subsections)
                .HasForeignKey(s => s.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional: Add indexes for better performance
            modelBuilder.Entity<Article>()
                .HasIndex(a => a.CategoryId);

            modelBuilder.Entity<Subsection>()
                .HasIndex(s => s.ArticleId);


            modelBuilder.Entity<User>()
                .HasMany(u => u.Committees)
                .WithMany(c => c.Users)
                .UsingEntity(j => j.ToTable("UserCommittees"));


        }

        // public DbSet<User> Users { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Users_Tasks> Users_Tasks { get; set; }
        public DbSet<Users_Meetings> Users_Meetings { get; set; }
        public DbSet<Committee> Committees { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Subsection> Subsections { get; set; }

        public DbSet<RefreshToken> RefreshToken { get; set; }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }

    }
}