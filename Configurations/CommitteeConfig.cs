using IEEE.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace IEEE.Configurations
{
    public class CommitteeConfig : IEntityTypeConfiguration<Committee> 
    {
        public void Configure(EntityTypeBuilder<Committee> builder)
        {
       
            builder.HasMany(c => c.Tasks)         
                   .WithOne(t => t.Committee)   
                   .HasForeignKey(t => t.CommitteeId) 
                   .OnDelete(DeleteBehavior.Cascade); 


        }
    }

}
