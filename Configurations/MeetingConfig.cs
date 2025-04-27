using IEEE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IEEE.Configurations
{
    public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
    {
        public void Configure(EntityTypeBuilder<Meeting> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id).ValueGeneratedOnAdd();
            builder.Property(m => m.Title).IsRequired();
            builder.Property(m => m.Description).IsRequired();
            builder.Property(m => m.Recap).IsRequired();

            // علاقة Meeting مع Committee (many-to-one)
            builder.HasOne(m => m.Committee)
                .WithMany(c => c.Meetings)
                .HasForeignKey(m => m.CommitteeId)
                .OnDelete(DeleteBehavior.Cascade);

            // علاقة Meeting مع Users (many-to-many)
            builder.HasMany(m => m.Users)
                .WithMany(u => u.Meetings)
                .UsingEntity<MeetingUser>(
                    j => j.HasOne(mu => mu.User)
                          .WithMany(u => u.MeetingUsers)
                          .HasForeignKey(mu => mu.UserId),
                    j => j.HasOne(mu => mu.Meeting)
                          .WithMany(m => m.MeetingUsers)
                          .HasForeignKey(mu => mu.MeetingId),
                    j =>
                    {
                        j.HasKey(mu => new { mu.MeetingId, mu.UserId });
                    });
        }
    }
}
