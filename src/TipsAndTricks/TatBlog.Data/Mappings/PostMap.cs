using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TatBlog.Core.Entities;

namespace TatBlog.Data.Mappings
{
    public class PostMap : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(p => p.ShortDescription)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(p => p.UrlSlug)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Meta)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(p => p.ImageUrl)
               .HasMaxLength(1000);

            builder.Property(p => p.ViewCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.Published)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.PostDate)
                .HasColumnType("datetime");

            builder.Property(p => p.ModifedDate)
                .HasColumnType("datetime");

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryID)
                .HasConstraintName("FK_Posts_Categories")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Author)
                .WithMany(a => a.Posts)
                .HasForeignKey(p => p.AuthorID)
                .HasConstraintName("FK_Posts_Authors")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Tags)
                .WithMany(t => t.Posts)
                .UsingEntity(pt => pt.ToTable("PostTags"));
        }
    }
}
