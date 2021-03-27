﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PMAuth.AuthDbContext;

namespace PMAuth.Migrations
{
    [DbContext(typeof(BackOfficeContext))]
    partial class BackOfficeContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("PMAuth.AuthDbContext.Entities.Admin", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("PMAuth.AuthDbContext.Entities.App", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AdminId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AdminId");

                    b.ToTable("Apps");
                });

            modelBuilder.Entity("PMAuth.AuthDbContext.Entities.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("AppId")
                        .HasColumnType("integer");

                    b.Property<string>("ClientId")
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Scope")
                        .HasColumnType("text");

                    b.Property<string>("SecretKey")
                        .HasColumnType("text");

                    b.Property<int>("SocialId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AppId");

                    b.HasIndex("SocialId");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("PMAuth.AuthDbContext.Entities.Social", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("AuthUri")
                        .HasColumnType("text");

                    b.Property<string>("LogoPath")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("TokenUrl")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Socials");
                });

            modelBuilder.Entity("PMAuth.AuthDbContext.Entities.App", b =>
                {
                    b.HasOne("PMAuth.AuthDbContext.Entities.Admin", "Admin")
                        .WithMany()
                        .HasForeignKey("AdminId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Admin");
                });

            modelBuilder.Entity("PMAuth.AuthDbContext.Entities.Setting", b =>
                {
                    b.HasOne("PMAuth.AuthDbContext.Entities.App", "App")
                        .WithMany()
                        .HasForeignKey("AppId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PMAuth.AuthDbContext.Entities.Social", "Social")
                        .WithMany()
                        .HasForeignKey("SocialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("App");

                    b.Navigation("Social");
                });
#pragma warning restore 612, 618
        }
    }
}
