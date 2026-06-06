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
    public class GymUserConfig<T> : IEntityTypeConfiguration<T> where T : GymUser
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(X => X.Name)
                   .HasColumnType("varchar")
                   .HasMaxLength(50);

            builder.Property(X => X.Email)
                   .HasColumnType("varchar")
                   .HasMaxLength(100);

            builder.ToTable(t => t.HasCheckConstraint("GymUserEmailConstraint", "Email like '%_@__%.__%' ")); // ahmed@gmail.com

            builder.Property(X => X.Phone)
                   .HasColumnType("varchar")
                   .HasMaxLength(11);
            builder.ToTable(t => t.HasCheckConstraint("GymUserEgyptianPhone",
                                    "Phone like '[0][1][0125][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' and len(phone) = 11"));

            builder.OwnsOne(x => x.Address, addr =>
            {
                addr.Property(x => x.City)
                    .HasColumnType("varchar")
                    .HasMaxLength(30);

                addr.Property(x => x.Street)
                    .HasColumnType("varchar")
                    .HasMaxLength(30);
            });
        }
    }
}
