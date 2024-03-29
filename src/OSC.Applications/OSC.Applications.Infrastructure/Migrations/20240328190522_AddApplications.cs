using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSC.Applications.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Application",
                table: "Application");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Application_Activity_Enum",
                table: "Application");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Application_Status_Enum",
                table: "Application");
            
            migrationBuilder.RenameTable(
                name: "Application",
                newName: "Applications");

            migrationBuilder.RenameIndex(
                name: "IX_Application_AuthorId",
                table: "Applications",
                newName: "IX_Applications_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Applications",
                table: "Applications",
                column: "Id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Applications_Activity_Enum",
                table: "Applications",
                sql: "\"Activity\" IN ('Report', 'Masterclass', 'Discussion')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Applications_Status_Enum",
                table: "Applications",
                sql: "\"Status\" IN ('Draft', 'Submitted')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Applications",
                table: "Applications");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Applications_Activity_Enum",
                table: "Applications");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Applications_Status_Enum",
                table: "Applications");

            migrationBuilder.RenameTable(
                name: "Applications",
                newName: "Application");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_AuthorId",
                table: "Application",
                newName: "IX_Application_AuthorId");
            
            migrationBuilder.AddPrimaryKey(
                name: "PK_Application",
                table: "Application",
                column: "Id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Application_Activity_Enum",
                table: "Application",
                sql: "\"Activity\" IN ('Report', 'Masterclass', 'Discussion')");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Application_AuthorId_MinLength",
                table: "Application",
                sql: "LENGTH(\"AuthorId\") >= 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Application_Status_Enum",
                table: "Application",
                sql: "\"Status\" IN ('Draft', 'Submitted')");
        }
    }
}
