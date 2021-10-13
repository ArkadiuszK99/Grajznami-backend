using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Entities;

namespace Persistance.Contexts
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.SportId)
                .IsRequired();
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(40);
            builder.Property(t => t.Place)
                .IsRequired()
                .HasMaxLength(60);
            builder.Property(t => t.Date)
                .IsRequired();
            builder.Property(t => t.Status)
                .IsRequired();
            builder.Property(t => t.OrganiserId)
                .IsRequired()
                .HasMaxLength(450);
            builder.Property(t => t.UsersLimit)
                .IsRequired();
            builder.Property(t => t.IsPublic)
                .IsRequired();
            builder.Property(t => t.IsFree)
                .IsRequired();
            builder.Property(t => t.TotalCost)
                .IsRequired().HasColumnType("decimal(18,4)");

            builder.HasOne(s => s.Sport)
            .WithMany(e => e.Events)
            .HasForeignKey(x => x.SportId)
            .IsRequired();

            builder.HasMany(u => u.Users)
            .WithMany(t => t.Events);

            builder.HasMany(u => u.InvitedUsers)
            .WithMany(t => t.InvitedToEvents);
        }
    }
}
