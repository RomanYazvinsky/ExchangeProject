using System;
using DatabaseModel.Constants;
using DatabaseModel.Entities;
using DatabaseModel.Entities.JoinEntities;
using Exchange.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Exchange
{
    public class ExchangeDbContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<MeasureUnitEntity> MeasureUnits { get; set; }
        public DbSet<MeasureUnitConversionEntity> MeasureUnitConversions { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductClassEntity> ProductClasses { get; set; }
        public DbSet<ProductClassAttributeEntity> ProductClassAttributes { get; set; }
        public DbSet<ProductClassAttributeValueEntity> ProductClassAttributeValues { get; set; }
        public DbSet<ValueChangeRequestEntity> ValueChangeRequests { get; set; }
        public DbSet<UserDeviceLoginEntity> UserDeviceLogins { get; set; }

        protected ExchangeDbContext()
        {
        }

        public ExchangeDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserEntity>(builder =>
            {
                builder.HasKey(user => user.Guid);
                builder.HasMany(user => user.Products)
                    .WithOne(product => product.Owner)
                    .HasForeignKey(product => product.UserId)
                    .IsRequired(false);
                builder.HasMany(user => user.UserDeviceLogins)
                    .WithOne(login => login.User)
                    .HasForeignKey(login => login.UserId)
                    .IsRequired();
                builder.Property(user => user.Email).IsRequired(false);
                builder.HasIndex(user => user.UserName).IsUnique();
                builder.Property(user => user.Role)
                    .IsRequired()
                    .HasConversion(new EnumToStringConverter<Role>());
                builder.HasData(new UserEntity
                {
                    Guid = Guid.NewGuid(),
                    UserName = "admin",
                    PasswordHash = PasswordUtil.HashPassword("admin"),
                    Role = Role.Administrator
                });
            });
            modelBuilder.Entity<ProductClassAttributeValueEntity>(builder =>
            {
                builder.HasKey(value => value.Guid);
                builder.HasIndex(e => new {e.ProductId, e.ProductClassAttributeId}).IsUnique();
                builder.HasOne(value => value.Product)
                    .WithMany(product => product.ProductClassAttributeValues)
                    .HasForeignKey(value => value.ProductId);
                builder.HasOne(value => value.ProductClassAttribute)
                    .WithMany(classAttribute => classAttribute.AttributeValues)
                    .HasForeignKey(value => value.ProductClassAttributeId);
            });
            modelBuilder.Entity<ProductClassAttributeEntity>(builder =>
            {
                builder.HasKey(value => value.Guid);
                builder.HasIndex(e => new {e.Name, e.AssociatedClassId}).IsUnique();
                builder.HasOne(pca => pca.AssociatedClass)
                    .WithMany(pc => pc.ProductClassAttributes)
                    .HasForeignKey(attribute => attribute.AssociatedClassId);
                builder.HasMany(pca => pca.AttributeValues)
                    .WithOne(attributeValue => attributeValue.ProductClassAttribute)
                    .HasForeignKey(attribute => attribute.ProductClassAttributeId);
                builder.Property(attribute => attribute.Mandatory).IsRequired().HasDefaultValue(false);
                builder.Property(attribute => attribute.ValueDataType).IsRequired().HasDefaultValue(ValueDataType.Text);
            });
            modelBuilder.Entity<ProductClassEntity>(builder => { builder.HasKey(value => value.Name); });
            modelBuilder.Entity<ProductEntity>(builder => { builder.HasKey(value => value.Guid); });
            modelBuilder.Entity<MeasureUnitEntity>(builder =>
            {
                builder.HasKey(value => value.Guid);
                builder.Property(unit => unit.Name).IsRequired();
                builder.Property(unit => unit.ShortName).IsRequired();
            });
            modelBuilder.Entity<MeasureUnitConversionEntity>(builder =>
            {
                builder.HasKey(value => new {value.FromId, value.ToId});
                builder.HasOne(value => value.From)
                    .WithMany(unit => unit.Conversions)
                    .HasForeignKey(value => value.FromId);
            });
            modelBuilder.Entity<ValueChangeRequestEntity>(builder =>
            {
                builder.HasKey(value => value.Guid);
                builder.HasOne(value => value.Sender)
                    .WithMany(user => user.ChangeRequests)
                    .HasForeignKey(value => value.SenderId);
                builder.HasOne(value => value.AttributeValue)
                    .WithMany(value => value.ChangeRequests)
                    .HasForeignKey(request => request.ProductClassAttributeValueId);
            });
            modelBuilder.Entity<ProductProductClassEntity>(builder =>
            {
                builder.HasKey(ppc => new {ppc.ProductId, ppc.ProductClassId});
                builder.HasOne(ppc => ppc.Product)
                    .WithMany(product => product.ProductProductClasses)
                    .HasForeignKey(ppc => ppc.ProductId);
                builder.HasOne(ppc => ppc.ProductClass)
                    .WithMany(pc => pc.ProductProductClasses)
                    .HasForeignKey(ppc => ppc.ProductClassId);
            });
            modelBuilder.Entity<UserDeviceLoginEntity>(builder => builder.HasKey(userDeviceLogin => userDeviceLogin.Guid));
        }
    }
}
