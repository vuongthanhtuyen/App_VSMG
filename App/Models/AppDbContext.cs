﻿using App.Models.Blog;
using App.Models.Contacts;
using App.Models.Product;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace App.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            foreach(var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
            // Nhấn chỉ mục để làm url trang web
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique() ;
            });
            // Thiếp lập khóa chính cho bản PostCategory
            modelBuilder.Entity<PostCategory>(entity =>
            {
                entity.HasKey(c => new { c.PostID, c.CategoryID });
            });



            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasIndex(c => c.Slug)
                .IsUnique(); //Column names in each table must be unique. Column name 'PostId' in table 'PostCategory' is specified more than once.
            });
            // Tạo dannh mục các sản phẩm
            modelBuilder.Entity<CategoryProduct>(entity => // là đối tượng biểu diển category trong cơ sở dữ liệu
            {
                entity.HasIndex(c => c.Slug).IsUnique(); // tạo chỉ mục 
            });

            modelBuilder.Entity<ProductCategoryProduct>(entity => {
                entity.HasKey(c => new { c.ProductID, c.CategoryID });
            }); // thiết lập khóa chính là Postid và categoryid

            modelBuilder.Entity<ProductModel>(entity => {
                entity.HasIndex(p => p.Slug)
                      .IsUnique(); // Không có 2 bài post có chỉ mục giống nhau
            });


        }


        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategoties { get; set; }

        public DbSet<CategoryProduct> CategoryProducts { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<ProductCategoryProduct> ProductCategoryProducts { get; set; }


        // Upload ảnh:
        public DbSet<ProductPhoto> ProductPhotos { get; set; }

    }
}
