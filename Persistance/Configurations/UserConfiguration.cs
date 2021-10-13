using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Persistance.Configurations
{
    class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(13); // sprawdziłem, takie jest najdłuższe imię w PL :P
            builder.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(26); // jw
            builder.Property(t => t.DateOfBirth)
                .IsRequired();
            builder.Property(t => t.Description)
                .HasMaxLength(200);
            builder.Property(t => t.Email)
                .IsRequired().HasMaxLength(50);
            builder.Property(t => t.PhoneNumber)
                .HasMaxLength(14); // tak na oko, są jakieś zagraniczne też które mają inny format niż 9 cyfrowy

            builder.HasMany(u => u.Events)
                .WithMany(t => t.Users);

            builder.HasMany(u => u.InvitedToEvents)
            .WithMany(t => t.InvitedUsers);
        }
    }
}
