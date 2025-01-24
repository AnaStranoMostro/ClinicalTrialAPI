﻿// <auto-generated />
using System;
using ClinicalTrialAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ClinicalTrialAPI.Migrations
{
    [DbContext(typeof(ClinicalTrialAPIContext))]
    [Migration("20250123220752_DurationField")]
    partial class DurationField
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ClinicalTrialAPI.Models.ClinicalTrialMetadata", b =>
                {
                    b.Property<string>("TrialId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Participants")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TrialDuration")
                        .HasColumnType("int");

                    b.HasKey("TrialId");

                    b.ToTable("ClinicalTrialMetadata");
                });
#pragma warning restore 612, 618
        }
    }
}
