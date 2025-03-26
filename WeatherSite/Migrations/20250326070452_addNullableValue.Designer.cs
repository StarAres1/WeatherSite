﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WeatherSite.Data;

#nullable disable

namespace WeatherSite.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250326070452_addNullableValue")]
    partial class addNullableValue
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WeatherSite.Models.Report", b =>
                {
                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<TimeOnly>("Time")
                        .HasColumnType("time without time zone");

                    b.Property<byte?>("CloudCover")
                        .HasColumnType("smallint");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("DirectionWind")
                        .HasColumnType("text");

                    b.Property<int>("H")
                        .HasColumnType("integer");

                    b.Property<byte?>("Humidity")
                        .HasColumnType("smallint");

                    b.Property<int>("Pressure")
                        .HasColumnType("integer");

                    b.Property<short>("Td")
                        .HasColumnType("smallint");

                    b.Property<short>("Temperature")
                        .HasColumnType("smallint");

                    b.Property<byte?>("VV")
                        .HasColumnType("smallint");

                    b.Property<byte?>("VelocityWind")
                        .HasColumnType("smallint");

                    b.HasKey("Date", "Time");

                    b.ToTable("Reports");
                });
#pragma warning restore 612, 618
        }
    }
}
