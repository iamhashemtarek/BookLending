using BookLending.Domain.Entities;
using BookLending.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Infrastructure.Persistence.Configuration
{
    public class BorrowConfiguration : IEntityTypeConfiguration<Borrow>
    {
        public void Configure(EntityTypeBuilder<Borrow> builder)
        {
            builder
            .HasKey(b => b.Id);

            builder.Property(o => o.Status).IsRequired().HasConversion<string>();

            builder
                .HasOne(b => b.Book)
                .WithMany(book => book.Borrows)
                .HasForeignKey(b => b.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(b => b.User)
                .WithMany(user => user.Borrows)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
