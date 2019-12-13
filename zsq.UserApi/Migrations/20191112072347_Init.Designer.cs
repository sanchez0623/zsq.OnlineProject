﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using zsq.UserApi.Data;

namespace zsq.UserApi.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20191112072347_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("zsq.UserApi.Models.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("Avatar");

                    b.Property<string>("City");

                    b.Property<int>("CityId");

                    b.Property<string>("Company");

                    b.Property<string>("Email");

                    b.Property<byte>("Gender");

                    b.Property<string>("Name");

                    b.Property<string>("NameCard");

                    b.Property<string>("Phone");

                    b.Property<string>("Province");

                    b.Property<int>("ProvinceId");

                    b.Property<string>("Tel");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("zsq.UserApi.Models.BPFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AppUserId");

                    b.Property<DateTime>("CreateTime");

                    b.Property<string>("FileName");

                    b.Property<string>("FormatFilePath");

                    b.Property<string>("OriginFilePath");

                    b.HasKey("Id");

                    b.ToTable("UserBPFiles");
                });

            modelBuilder.Entity("zsq.UserApi.Models.UserProperty", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(100);

                    b.Property<string>("Text")
                        .HasMaxLength(100);

                    b.Property<string>("Value")
                        .HasMaxLength(100);

                    b.Property<int>("AppUserId");

                    b.HasKey("Key", "Text", "Value");

                    b.HasIndex("AppUserId");

                    b.ToTable("UserProperties");
                });

            modelBuilder.Entity("zsq.UserApi.Models.UserTag", b =>
                {
                    b.Property<int>("AppUserId");

                    b.Property<string>("Tag")
                        .HasMaxLength(100);

                    b.Property<DateTime>("CreateTime");

                    b.HasKey("AppUserId", "Tag");

                    b.ToTable("UserTags");
                });

            modelBuilder.Entity("zsq.UserApi.Models.UserProperty", b =>
                {
                    b.HasOne("zsq.UserApi.Models.AppUser")
                        .WithMany("Properties")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
