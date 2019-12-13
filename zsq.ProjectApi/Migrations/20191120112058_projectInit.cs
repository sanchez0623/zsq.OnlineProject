﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace zsq.ProjectApi.Migrations
{
    public partial class projectInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    UserId = table.Column<int>(nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    Company = table.Column<string>(nullable: true),
                    OriginBPFile = table.Column<string>(nullable: true),
                    FormatBPFile = table.Column<string>(nullable: true),
                    ShowSecurityInfo = table.Column<short>(nullable: false),
                    ProvinceId = table.Column<int>(nullable: false),
                    Province = table.Column<string>(nullable: true),
                    CityId = table.Column<int>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    AreaId = table.Column<int>(nullable: false),
                    Area = table.Column<string>(nullable: true),
                    RegisterTime = table.Column<DateTime>(nullable: false),
                    Introduction = table.Column<string>(nullable: true),
                    FinPercentage = table.Column<string>(nullable: true),
                    FinStage = table.Column<string>(nullable: true),
                    FinMoney = table.Column<string>(nullable: true),
                    Income = table.Column<int>(nullable: false),
                    Revenue = table.Column<int>(nullable: false),
                    Valuation = table.Column<int>(nullable: false),
                    BrokerageOptions = table.Column<int>(nullable: false),
                    OnPlatform = table.Column<short>(nullable: false),
                    SourceId = table.Column<int>(nullable: false),
                    ReferenceId = table.Column<int>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: false),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectContributors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ProjectId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false),
                    IsCloser = table.Column<short>(nullable: false),
                    ContributorType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectContributors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectContributors_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectProperty",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<string>(maxLength: 100, nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProperty", x => new { x.ProjectId, x.Key, x.Value });
                    table.ForeignKey(
                        name: "FK_ProjectProperty_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectViewer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ProjectId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    CreateTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectViewer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectViewer_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectVisibleRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ProjectId = table.Column<int>(nullable: false),
                    Visible = table.Column<short>(nullable: false),
                    Tags = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectVisibleRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectVisibleRules_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContributors_ProjectId",
                table: "ProjectContributors",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViewer_ProjectId",
                table: "ProjectViewer",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectVisibleRules_ProjectId",
                table: "ProjectVisibleRules",
                column: "ProjectId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectContributors");

            migrationBuilder.DropTable(
                name: "ProjectProperty");

            migrationBuilder.DropTable(
                name: "ProjectViewer");

            migrationBuilder.DropTable(
                name: "ProjectVisibleRules");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
