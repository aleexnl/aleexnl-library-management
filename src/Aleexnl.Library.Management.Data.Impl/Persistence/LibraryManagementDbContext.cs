using Aleexnl.Library.Management.Data.Contracts.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aleexnl.Library.Management.Data.Impl.Persistence;

/// <summary>
/// EF Core database context for the library management application.
/// </summary>
/// <param name="options">The options used to configure the context.</param>
public sealed class LibraryManagementDbContext(DbContextOptions<LibraryManagementDbContext> options)
    : DbContext(options)
{
    /// <summary>
    /// Gets the books set.
    /// </summary>
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<Book>(builder =>
        {
            builder.ToTable("Books");
            builder.HasKey(book => book.Id);

            builder.Property(book => book.Title)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(book => book.Author)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(book => book.Isbn)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(book => book.NormalizedIsbn)
                .HasMaxLength(32)
                .IsRequired();

            builder.Property(book => book.Description)
                .HasMaxLength(2000);

            builder.Property(book => book.CreatedAtUtc)
                .IsRequired();

            builder.HasIndex(book => book.NormalizedIsbn)
                .IsUnique()
                .HasFilter("\"IsDeleted\" = 0");

            builder.HasQueryFilter("SoftDeletionFilter", book => !book.IsDeleted);
        });
}
