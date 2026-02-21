using ASP_NET_Final_Proj.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP_NET_Final_Proj.Data;

public class InvoiceManagerDbContext : IdentityDbContext<User>
{
    public InvoiceManagerDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<InvoiceRow> InvoiceRows => Set<InvoiceRow>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer
        modelBuilder.Entity<Customer>(
            entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(c => c.Address)
                    .HasMaxLength(200);
                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(300);
                entity.Property(c => c.PhoneNumber)
                    .HasMaxLength(20);
                entity.Property(c => c.CreatedAt)
                    .IsRequired();
                entity.HasOne(c => c.User)
                    .WithMany(u => u.Customers)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        // Invoice
        modelBuilder.Entity<Invoice>(
            entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.StartDate)
                    .IsRequired();
                entity.Property(i => i.EndDate)
                    .IsRequired();
                entity.Property(i => i.TotalSum)
                    .IsRequired()
                    .HasPrecision(18, 4);
                entity.Property(i => i.Status)
                    .IsRequired();
                entity.Property(i => i.Comment)
                    .HasMaxLength(300);
                entity.Property(i => i.CreatedAt)
                    .IsRequired();

                entity.HasOne(i => i.Customer)
                    .WithMany(c => c.Invoices)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        // InvoiceRow
        modelBuilder.Entity<InvoiceRow>(
            entity =>
            {
                entity.HasKey(ir => ir.Id);
                entity.Property(ir => ir.Service)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(ir => ir.Quantity)
                    .IsRequired()
                    .HasPrecision(18, 4);
                entity.Property(ir => ir.Amount)
                    .IsRequired()
                    .HasPrecision(18, 4);
                entity.Property(ir => ir.Sum)
                    .IsRequired()
                    .HasPrecision(18, 4);

                entity.HasOne(ir => ir.Invoice)
                    .WithMany(i => i.Rows)
                    .HasForeignKey(e => e.InvoiceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

        // RefreshToken
        modelBuilder.Entity<RefreshToken>(
            entity =>
            {
                entity
                    .HasKey(e => e.Id);

                entity
                    .HasIndex(e => e.JwtId)
                    .IsUnique();

                entity
                    .Property(e => e.JwtId)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

            });
    }

}
