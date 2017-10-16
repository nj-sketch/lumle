using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Lumle.Web.Migrations
{
    public partial class UserProfileUpdateSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calendar_Event");

            migrationBuilder.DropTable(
                name: "Calendar_Holiday");

            migrationBuilder.DropColumn(
                name: "AboutMe",
                table: "Authorization_UserProfile");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Authorization_UserProfile");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Authorization_UserProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Authorization_UserProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Authorization_UserProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Authorization_UserProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreetAddress",
                table: "Authorization_UserProfile",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "Authorization_UserProfile");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Authorization_UserProfile");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Authorization_UserProfile");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Authorization_UserProfile");

            migrationBuilder.DropColumn(
                name: "StreetAddress",
                table: "Authorization_UserProfile");

            migrationBuilder.AddColumn<string>(
                name: "AboutMe",
                table: "Authorization_UserProfile",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Authorization_UserProfile",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Calendar_Event",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Remarks = table.Column<string>(maxLength: 250, nullable: true),
                    Start = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendar_Event", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Calendar_Holiday",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Remarks = table.Column<string>(maxLength: 250, nullable: true),
                    Title = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendar_Holiday", x => x.Id);
                });
        }
    }
}
