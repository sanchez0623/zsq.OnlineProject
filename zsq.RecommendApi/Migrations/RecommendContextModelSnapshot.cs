﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using zsq.RecommendApi.Data;

namespace zsq.RecommendApi.Migrations
{
    [DbContext(typeof(RecommendContext))]
    partial class RecommendContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity("zsq.RecommendApi.Models.ProjectRecommend", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreateTime");

                    b.Property<string>("FromUserAvatar");

                    b.Property<int>("FromUserId");

                    b.Property<string>("FromUserName");

                    b.Property<string>("ProjectAvatar");

                    b.Property<string>("ProjectCompany");

                    b.Property<string>("ProjectFinStage");

                    b.Property<int>("ProjectId");

                    b.Property<string>("ProjectIntroduction");

                    b.Property<string>("ProjectTags");

                    b.Property<DateTime>("RecommendTime");

                    b.Property<int>("RecommendType");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.ToTable("ProjectRecommends");
                });
#pragma warning restore 612, 618
        }
    }
}
