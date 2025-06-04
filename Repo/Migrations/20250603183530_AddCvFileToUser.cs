using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repo.Migrations
{
    /// <inheritdoc />
    public partial class AddCvFileToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "cvfileid",
                table: "AspNetUsers",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_cvfileid",
                table: "AspNetUsers",
                column: "cvfileid");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_files_cvfileid",
                table: "AspNetUsers",
                column: "cvfileid",
                principalTable: "files",
                principalColumn: "file_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_files_cvfileid",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_cvfileid",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "cvfileid",
                table: "AspNetUsers");
        }
    }
}
