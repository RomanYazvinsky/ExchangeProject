using System;
using Exchange.Common.Utils;
using Exchange.Data.Constants;
using Exchange.Data.Entities;
using Exchange.Data.Entities.Currency;
using Exchange.Data.Entities.JoinEntities;
using Exchange.Data.Entities.Order;
using Exchange.Data.Entities.Seller;
using Exchange.Data.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Exchange.Data
{
    public class ExchangeDbContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<SellerEntity> Sellers { get; set; }
        public DbSet<AddressEntity> Addresses { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderTransactionEntity> OrderTransactions { get; set; }
        public DbSet<CurrencyEntity> Currencies { get; set; }
        public DbSet<MeasureUnitEntity> MeasureUnits { get; set; }
        public DbSet<MeasureUnitConversionEntity> MeasureUnitConversions { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductClassEntity> ProductClasses { get; set; }
        public DbSet<ProductClassAttributeEntity> ProductClassAttributes { get; set; }
        public DbSet<ProductClassAttributeValueEntity> ProductClassAttributeValues { get; set; }
        public DbSet<ValueChangeRequestEntity> ValueChangeRequests { get; set; }
        public DbSet<UserBillModifierEntity> UserBillModifiers { get; set; }
        public DbSet<SellerBillModifierEntity> SellerBillModifiers { get; set; }

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
                builder.HasKey(user => user.Id);
                builder.Property(user => user.Email).IsRequired(false);
                builder.HasIndex(user => user.Username).IsUnique();
                builder.HasIndex(user => user.Email).IsUnique();
                builder.Property(user => user.Role)
                    .IsRequired()
                    .HasConversion(new EnumToStringConverter<Role>());
                builder.HasData(new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    PasswordHash = PasswordUtil.HashPassword("passw0rd"),
                    Role = Role.Administrator,
                    Email = "admin@site-exchange.com",
                    IsEmailConfirmed = true
                });
            });
            modelBuilder.Entity<ProductClassAttributeValueEntity>(builder =>
            {
                builder.HasKey(value => value.Id);
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
                builder.HasKey(value => value.Id);
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
            modelBuilder.Entity<ProductEntity>(builder => { builder.HasKey(value => value.Id); });
            modelBuilder.Entity<MeasureUnitEntity>(builder =>
            {
                builder.HasKey(value => value.Id);
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
                builder.HasKey(value => value.Id);
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
            modelBuilder.Entity<SellerEntity>(builder =>
            {
                builder.HasKey(entity => entity.Id);
                builder.HasMany(seller => seller.Products)
                    .WithOne(product => product.Seller)
                    .HasForeignKey(product => product.SellerId)
                    .IsRequired(false);
                builder.HasOne(entity => entity.Address)
                    .WithOne()
                    .HasForeignKey<SellerEntity>(entity => entity.AddressId)
                    .IsRequired();
                builder.HasMany(entity => entity.Users)
                    .WithOne(entity => entity.Seller)
                    .HasForeignKey(user => user.SellerId);
                builder.Property(entity => entity.SellerName).IsRequired();
            });
            modelBuilder.Entity<OrderEntity>(builder =>
            {
                builder.HasKey(entity => entity.Id);
                builder.HasOne(entity => entity.Customer)
                    .WithMany(user => user.Orders)
                    .HasForeignKey(entity => entity.CustomerId)
                    .IsRequired();
                builder.HasOne(entity => entity.Product)
                    .WithMany(product => product.Orders)
                    .HasForeignKey(entity => entity.ProductId)
                    .IsRequired();
                builder.HasOne(entity => entity.Address)
                    .WithOne()
                    .HasForeignKey<OrderEntity>(order => order.AddressId);
                builder.HasMany(entity => entity.Modifiers)
                    .WithOne(modifier => modifier.Order)
                    .HasForeignKey(modifier => modifier.OrderId);
                builder.Property(entity => entity.Quantity).IsRequired();
            });
            modelBuilder.Entity<OrderTransactionEntity>(builder =>
            {
                builder.HasKey(entity => entity.Id);
                builder.HasOne(entity => entity.Order)
                    .WithMany(order => order.Transactions)
                    .HasForeignKey(entity => entity.OrderId);
                builder.HasOne(entity => entity.Currency)
                    .WithMany()
                    .HasForeignKey(entity => entity.CurrencyId);
            });
            modelBuilder.Entity<BillModifierEntity>(builder =>
            {
                builder.HasKey(entity => entity.Id);
                builder.HasDiscriminator();
            });
            modelBuilder.Entity<SellerBillModifierEntity>(builder =>
            {
                builder.HasOne(entity => entity.Product)
                    .WithOne(product => product.Discount)
                    .HasForeignKey<SellerBillModifierEntity>(entity => entity.ProductId);
                builder.HasOne(entity => entity.Seller)
                    .WithMany(seller => seller.Discounts)
                    .HasForeignKey(entity => entity.OwnerId);
            });
            modelBuilder.Entity<UserBillModifierEntity>(builder =>
            {
                builder.HasOne(entity => entity.User)
                    .WithMany(seller => seller.Discounts)
                    .HasForeignKey(entity => entity.OwnerId);
            });
            modelBuilder.Entity<BillModifierEntity>(builder =>
            {
                builder.HasKey(entity => entity.Id);
                builder.HasDiscriminator();
            });
            modelBuilder.Entity<CurrencyEntity>(builder =>
            {
                builder.HasKey(entity => entity.Id);
                builder.HasData(new CurrencyEntity
                {
                    Id = Guid.NewGuid(),
                    Abbreviation = "USD",
                    CountryCode = "840",
                    CurrencySign = "$",
                    Name = "United States Dollar"
                });
            });
            modelBuilder.Entity<AddressEntity>().HasKey(entity => entity.Id);
            modelBuilder.Entity<OrderToBillModifier>().HasKey(entity => new { entity.OrderId, entity.BillModifierId});
        }
    }
}
