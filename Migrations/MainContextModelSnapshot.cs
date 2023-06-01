﻿// <auto-generated />
using System;
using DS_Test.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DS_Test.Migrations
{
    [DbContext(typeof(MainContext))]
    partial class MainContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("DS_Test.Models.WeatherRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AirFlowDirection")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("AirFlowSpeed")
                        .HasColumnType("float");

                    b.Property<double?>("AirHumidity")
                        .HasColumnType("float");

                    b.Property<double?>("Cloudiness")
                        .HasColumnType("float");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<double?>("Pressure")
                        .HasColumnType("float");

                    b.Property<double?>("Td")
                        .HasColumnType("float");

                    b.Property<double?>("Temperature")
                        .HasColumnType("float");

                    b.Property<double?>("VV")
                        .HasColumnType("float");

                    b.Property<string>("WeatherEvents")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("h")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("WeatherRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
