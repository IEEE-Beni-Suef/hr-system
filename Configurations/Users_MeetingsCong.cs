using IEEE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IEEE.Configurations
{
    public class Users_MeetingsCong : IEntityTypeConfiguration<Users_Meetings>
    {
        public void Configure(EntityTypeBuilder<Users_Meetings> builder)
        {
            // مفتاح مركب
            builder.HasKey(x => new { x.UserId, x.MeetingId });

            // العلاقة مع User
            builder.HasOne(um => um.User)
                .WithMany(u => u.Users_Meetings)
                .HasForeignKey(um => um.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // العلاقة مع Meeting
            builder.HasOne(um => um.Meeting)
                .WithMany(m => m.Users_Meetings)
                .HasForeignKey(um => um.MeetingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
