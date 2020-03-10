using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Restaurant.Infrastructure.Models;

namespace Restaurant.Infrastructure.Context
{
    public partial class fastfood_dbContext : DbContext
    {
        public fastfood_dbContext(DbContextOptions<fastfood_dbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<CartItem> CartItem { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<CategoryPortion> CategoryPortion { get; set; }
        public virtual DbSet<Image> Image { get; set; }
        public virtual DbSet<Ingredient> Ingredient { get; set; }
        public virtual DbSet<Item> Item { get; set; }
        public virtual DbSet<ItemIngredient> ItemIngredient { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Portion> Portion { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString, x => x.ServerVersion("8.0.12-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("cart");

                entity.HasIndex(e => e.UserId)
                    .HasName("uq_cart_user_id")
                    .IsUnique();

                entity.Property(e => e.CartId)
                    .HasColumnName("cart_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Cart)
                    .HasForeignKey<Cart>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_cart_user_id");
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("cart_item");

                entity.HasIndex(e => e.CartId)
                    .HasName("fk_cart_item_cart_id");

                entity.HasIndex(e => e.ItemId)
                    .HasName("fk_cart_item_item_id");

                entity.Property(e => e.CartItemId)
                    .HasColumnName("cart_item_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("int(4) unsigned");

                entity.Property(e => e.CartId)
                    .HasColumnName("cart_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.ItemId)
                    .HasColumnName("item_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.CartItem)
                    .HasForeignKey(d => d.CartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_cart_item_cart_id");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.CartItem)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_cart_item_item_id");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("category");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.IsScalable).HasColumnName("is_scalable");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("varchar(30)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CategoryPortion>(entity =>
            {
                entity.ToTable("category_portion");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("fk_category_portion_category_id");

                entity.HasIndex(e => e.PortionId)
                    .HasName("fk_category_portion_portion_id_idx");

                entity.Property(e => e.CategoryPortionId)
                    .HasColumnName("category_portion_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.PortionId)
                    .HasColumnName("portion_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.CategoryPortion)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_category_portion_category_id");

                entity.HasOne(d => d.Portion)
                    .WithMany(p => p.CategoryPortion)
                    .HasForeignKey(d => d.PortionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_category_portion_portion_id");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("image");

                entity.HasIndex(e => e.ItemId)
                    .HasName("fk_image_item_id");

                entity.HasIndex(e => e.Path)
                    .HasName("uq_image_path")
                    .IsUnique();

                entity.Property(e => e.ImageId)
                    .HasColumnName("image_id")
                    .HasColumnType("bigint(10) unsigned");

                entity.Property(e => e.ItemId)
                    .HasColumnName("item_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasColumnName("path")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Image)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_image_item_id");
            });

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("ingredient");

                entity.Property(e => e.IngredientId)
                    .HasColumnName("ingredient_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.Allergens)
                    .IsRequired()
                    .HasColumnName("allergens")
                    .HasColumnType("enum('','soja','gluten','orašasti plodovi','pečurke','morski plodovi')")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.IsBase)
                    .HasColumnName("is_base")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("item");

                entity.HasIndex(e => e.CategoryId)
                    .HasName("fk_item_category_id");

                entity.HasIndex(e => e.Title)
                    .HasName("uq_item_title")
                    .IsUnique();

                entity.Property(e => e.ItemId)
                    .HasColumnName("item_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.CalorieCount)
                    .HasColumnName("calorie_count")
                    .HasColumnType("int(4) unsigned");

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasColumnType("tinyint(1) unsigned");

                entity.Property(e => e.Mass)
                    .HasColumnName("mass")
                    .HasColumnType("int(4) unsigned");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(10,2) unsigned");

                entity.Property(e => e.RequestAntiForgeryToken)
                    .HasColumnName("request_anti_forgery_token")
                    .HasColumnType("varchar(44)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Item)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_item_category_id");
            });

            modelBuilder.Entity<ItemIngredient>(entity =>
            {
                entity.ToTable("item_ingredient");

                entity.HasIndex(e => e.IngredientId)
                    .HasName("fk_item_ingredient_ingredient_id");

                entity.HasIndex(e => e.ItemId)
                    .HasName("fk_item_ingredient_item_id");

                entity.Property(e => e.ItemIngredientId)
                    .HasColumnName("item_ingredient_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.IngredientId)
                    .HasColumnName("ingredient_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.ItemId)
                    .HasColumnName("item_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.HasOne(d => d.Ingredient)
                    .WithMany(p => p.ItemIngredient)
                    .HasForeignKey(d => d.IngredientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_item_ingredient_ingredient_id");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.ItemIngredient)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_item_ingredient_item_id");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("order");

                entity.HasIndex(e => e.CartId)
                    .HasName("fk_order_cart_id");

                entity.HasIndex(e => e.UserId)
                    .HasName("uq_order_user_id")
                    .IsUnique();

                entity.Property(e => e.OrderId)
                    .HasColumnName("order_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.CartId)
                    .HasColumnName("cart_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.DeliveredAt)
                    .HasColumnName("delivered_at")
                    .HasColumnType("time");

                entity.Property(e => e.DeliveryAt)
                    .HasColumnName("delivery_at")
                    .HasColumnType("time(4)");

                entity.Property(e => e.IsAccepted)
                    .IsRequired()
                    .HasColumnName("is_accepted")
                    .HasColumnType("enum('yes','no','pending')")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.IsCanceled)
                    .HasColumnName("is_canceled")
                    .HasColumnType("tinyint(1) unsigned");

                entity.Property(e => e.IsDelivered)
                    .HasColumnName("is_delivered")
                    .HasColumnType("tinyint(1) unsigned");

                entity.Property(e => e.PersonalPreference)
                    .HasColumnName("personal_preference")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.Rating)
                    .HasColumnName("rating")
                    .HasColumnType("int(1) unsigned");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.CartId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_order_cart_id");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Order)
                    .HasForeignKey<Order>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_order_user_id");
            });

            modelBuilder.Entity<Portion>(entity =>
            {
                entity.ToTable("portion");

                entity.Property(e => e.PortionId)
                    .HasColumnName("portion_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.MassCalorieMultiplier)
                    .HasColumnName("mass_calorie_multiplier")
                    .HasColumnType("decimal(3,2) unsigned");

                entity.Property(e => e.PriceMultiplier)
                    .HasColumnName("price_multiplier")
                    .HasColumnType("decimal(3,2) unsigned");

                entity.Property(e => e.SizeName)
                    .IsRequired()
                    .HasColumnName("size_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.Email)
                    .HasName("uq_user_email")
                    .IsUnique();

                entity.HasIndex(e => e.Username)
                    .HasName("uq_user_username")
                    .IsUnique();

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("bigint(11) unsigned");

                entity.Property(e => e.Address1)
                    .IsRequired()
                    .HasColumnName("address1")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.Address2)
                    .HasColumnName("address2")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.Address3)
                    .HasColumnName("address3")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.IsDeleted)
                    .HasColumnName("is_deleted")
                    .HasColumnType("tinyint(1) unsigned");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.RequestAntiForgeryToken)
                    .HasColumnName("request_anti_forgery_token")
                    .HasColumnType("varchar(44)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasColumnName("role")
                    .HasColumnType("enum('administrator','dispatcher','customer')")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
