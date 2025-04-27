using IEEE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Threading.Tasks;



namespace IEEE.Configurations
{
    public class TaskConfigurations : IEntityTypeConfiguration<Tasks>
    {
        public void Configure(EntityTypeBuilder<Tasks> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            // ربط Task بـ Head
            builder.HasOne(t => t.Head)
                .WithMany(h => h.HeadTasks)  
                .HasForeignKey(t => t.HeadId);

          
            builder.HasOne(t => t.Committee)  
                .WithMany(c => c.Tasks)        
                .HasForeignKey(t => t.CommitteeId)  
                .OnDelete(DeleteBehavior.Cascade);  
        }
    }
}
