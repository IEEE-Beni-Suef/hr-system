using IEEE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IEEE.Configurations
{
    public class MeetingConfig : IEntityTypeConfiguration<meetings>
    {
        public void Configure(EntityTypeBuilder<meetings> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.HasOne(m => m.Creator)
                .WithMany(c => c.CreatorMeetings)
                .HasForeignKey(c => c.CreatorId);

            builder.HasMany(m => m.Users)
                .WithMany(u => u.Meetings)
                .UsingEntity<Dictionary<string, object>>(
                "Users_Meetings",
                j => j.HasOne<User>().WithMany().OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<meetings>().WithMany().OnDelete(DeleteBehavior.Restrict)
                );

        }
    }
}
