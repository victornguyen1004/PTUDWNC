using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;

namespace TatBlog.Data.Mappings
{
    public class SubscriberMap : IEntityTypeConfiguration<Subscriber>
    {
        public void Configure(EntityTypeBuilder<Subscriber> builder)
        {
            builder.ToTable("Subscribers");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Email)
           .IsRequired();

            builder.Property(p => p.Reason)
                .HasMaxLength(5000);

            builder.Property(p => p.Note)
                .HasMaxLength(500);

            builder.Property(a => a.DateSubscribe)
                .HasColumnType("datetime");

            builder.Property(a => a.DateUnsubscribe)
                .HasColumnType("datetime");
        }
    }
}