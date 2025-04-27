using IEEE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IEEE.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).ValueGeneratedOnAdd();
            builder.Property(u => u.FName).IsRequired();
            builder.Property(u => u.MName).IsRequired();
            builder.Property(u => u.LName).IsRequired();
            builder.Property(u => u.Email).IsRequired();
            builder.Property(u => u.Phone).IsRequired();
            builder.Property(u => u.Sex).IsRequired();
            builder.Property(u => u.Committee).IsRequired();
            builder.Property(u => u.City).IsRequired();
            builder.Property(u => u.BirthOfDate).IsRequired();
            builder.Property(u => u.IsActive).HasDefaultValue(false);
            builder.Property(u => u.Role).IsRequired();

            // علاقة User مع Meetings (many-to-many)
            builder.HasMany(u => u.Meetings)
                .WithMany(m => m.Users)
                .UsingEntity<MeetingUser>(
                    j => j.HasOne(mu => mu.Meeting)
                          .WithMany(m => m.MeetingUsers)
                          .HasForeignKey(mu => mu.MeetingId),
                    j => j.HasOne(mu => mu.User)
                          .WithMany(u => u.MeetingUsers)
                          .HasForeignKey(mu => mu.UserId),
                    j =>
                    {
                        j.HasKey(mu => new { mu.MeetingId, mu.UserId });
                    });
        }
    }
}
