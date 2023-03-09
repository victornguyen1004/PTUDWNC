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
    public class CommentMap : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {

            builder.ToTable("Comments");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.UserName)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(c => c.CommentTime)
                .HasColumnType("datetime");

            builder.Property(c => c.Active)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}