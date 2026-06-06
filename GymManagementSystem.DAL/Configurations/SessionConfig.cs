using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.DAL.Configurations
{
    public class SessionConfig : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {

            builder.ToTable(t =>
            {
                t.HasCheckConstraint("SessionCapacityConstraint", "Capacity between 1 and 25");
                t.HasCheckConstraint("SessionEndDateAfterStartDate", "EndDate > StartDate");
            });

            builder.HasOne(x => x.Trainer)
                   .WithMany(x => x.Sessions)
                   .HasForeignKey(x => x.TrainerId);

            builder.HasOne(x => x.Category)
                   .WithMany(x => x.Sessions)
                   .HasForeignKey(x => x.CategoryId);
        }
    }
}
