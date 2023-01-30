﻿// <auto-generated />
using Autentication.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Autentication.Infrastructure.Migrations
{
    [DbContext(typeof(AutenticationContext))]
    [Migration("20230129012917_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.2");

            modelBuilder.Entity("Autentication.Core.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Password")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .HasMaxLength(250)
                        .IsUnicode(false)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Password = "Lilian",
                            Username = "Snape"
                        });
                });
#pragma warning restore 612, 618
        }
    }
}