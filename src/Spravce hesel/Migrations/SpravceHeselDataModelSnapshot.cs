﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spravce_hesel.Data;

#nullable disable

namespace Spravce_hesel.Migrations
{
    [DbContext(typeof(SpravceHeselData))]
    partial class SpravceHeselDataModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Spravce_hesel.Models.Heslo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("DesifrovanaSluzba")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DesifrovaneHeslo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DesifrovaneJmeno")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Jmeno")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("Sifra")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("Sluzba")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("UzivatelskeId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Hesla");
                });

            modelBuilder.Entity("Spravce_hesel.Models.SdileneHeslo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("DesifrovanaSluzba")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DesifrovaneHeslo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DesifrovaneJmeno")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DocasnyStringProKlic")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Jmeno")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<bool>("Potvrzeno")
                        .HasColumnType("bit");

                    b.Property<int>("PuvodniHesloId")
                        .HasColumnType("int");

                    b.Property<byte[]>("Sifra")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("Sluzba")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("UzivatelskeId")
                        .HasColumnType("int");

                    b.Property<string>("UzivatelskeJmeno")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ZakladatelId")
                        .HasColumnType("int");

                    b.Property<string>("ZakladatelJmeno")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Zmeneno")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("SdilenaHesla");
                });

            modelBuilder.Entity("Spravce_hesel.Models.Uzivatel", b =>
                {
                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Heslo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("IV")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Jmeno")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Email");

                    b.ToTable("Uzivatele");
                });
#pragma warning restore 612, 618
        }
    }
}
