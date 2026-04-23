
using ApiEcommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// 🧱 INFRASTRUCTURE LAYER - Domain Entities
using ApiEcommerce.Domain.Entities;

public class ApplicationDbContext : IdentityDbContext<AplicationUser>
{
    // 📝 DbContext para gestionar la conexión y las entidades EF Core
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // 🛠️ Configuración del modelo EF Core para entidades Domain
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 📋 Configuración para Category (Domain Entity)
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
            entity.Property(c => c.CreationDate).IsRequired();
            
            // ⚠️ Configurar constructor privado para EF Core
            entity.Property(c => c.Id).ValueGeneratedOnAdd();
        });

        // 📦 Configuración para Product (Domain Entity) 
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.ProductId);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).HasMaxLength(500);
            entity.Property(p => p.Price).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(p => p.SKU).IsRequired().HasMaxLength(100);
            entity.Property(p => p.Stock).IsRequired();
            entity.Property(p => p.CategoryId).IsRequired();
            entity.Property(p => p.CreationDate).IsRequired();
            entity.Property(p => p.UpdateDate);
            entity.Property(p => p.ImgUrl).HasMaxLength(500);
            entity.Property(p => p.ImgUrlLocal).HasMaxLength(500);
            
            // ⚠️ Configurar constructor privado para EF Core
            entity.Property(p => p.ProductId).ValueGeneratedOnAdd();
        });
    }

    // 📊 DbSets para entidades de dominio
    public DbSet<Category> Categories { get; set;}
    public DbSet<Product> Products { get; set;}
}