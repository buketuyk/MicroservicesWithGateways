﻿// <auto-generated />
using System;
using CodeFirstMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CodeFirstMicroservice.Migrations
{
    [DbContext(typeof(TaskManagementContext))]
    partial class TaskManagementContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CodeFirstMicroservice.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Career"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Shopping"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Personal Development"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Health"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Finance"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Home"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Hobbies"
                        },
                        new
                        {
                            Id = 8,
                            Name = "Education"
                        },
                        new
                        {
                            Id = 9,
                            Name = "Family"
                        },
                        new
                        {
                            Id = 10,
                            Name = "Other"
                        });
                });

            modelBuilder.Entity("CodeFirstMicroservice.Models.Status", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Statuses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Backlog"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Todo"
                        },
                        new
                        {
                            Id = 3,
                            Name = "In Progress"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Test"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Done"
                        });
                });

            modelBuilder.Entity("CodeFirstMicroservice.Models.TaskItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AssignedUserId")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("StatusId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AssignedUserId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("StatusId");

                    b.ToTable("TaskItems");
                });

            modelBuilder.Entity("CodeFirstMicroservice.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CodeFirstMicroservice.Models.TaskItem", b =>
                {
                    b.HasOne("CodeFirstMicroservice.Models.User", "AssignedUser")
                        .WithMany("Tasks")
                        .HasForeignKey("AssignedUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CodeFirstMicroservice.Models.Category", "Category")
                        .WithMany("TaskItems")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CodeFirstMicroservice.Models.Status", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssignedUser");

                    b.Navigation("Category");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("CodeFirstMicroservice.Models.Category", b =>
                {
                    b.Navigation("TaskItems");
                });

            modelBuilder.Entity("CodeFirstMicroservice.Models.User", b =>
                {
                    b.Navigation("Tasks");
                });
#pragma warning restore 612, 618
        }
    }
}
