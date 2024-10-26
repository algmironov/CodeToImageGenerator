﻿// <auto-generated />
using System;
using CodeToImageGenerator.Web.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CodeToImageGenerator.Web.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241026194504_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0-rc.2.24474.1");

            modelBuilder.Entity("CodeToImageGenerator.Web.Models.StatisticEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsSuccessful")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ProgrammingLanguage")
                        .HasColumnType("TEXT");

                    b.Property<string>("Source")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Statistics");
                });
#pragma warning restore 612, 618
        }
    }
}
