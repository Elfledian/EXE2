using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repo.Migrations
{
    /// <inheritdoc />
    public partial class removeUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK__candidate__user___4CA06362",
                table: "candidates");

            migrationBuilder.DropForeignKey(
                name: "FK__companies__recru__52593CB8",
                table: "companies");

            migrationBuilder.DropForeignKey(
                name: "FK__notificat__user___2645B050",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK__ratings__reviewe__18EBB532",
                table: "ratings");

            migrationBuilder.DropForeignKey(
                name: "FK__recruiter__user___59FA5E80",
                table: "recruiters");

            migrationBuilder.DropForeignKey(
                name: "FK__verificat__user___1F98B2C1",
                table: "verifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK__users__B9BE370FA57EB208",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "(newid())",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK__users__B9BE370FA57EB208",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__candidate__user___4CA06362",
                table: "candidates",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__companies__recru__52593CB8",
                table: "companies",
                column: "recruiter_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__notificat__user___2645B050",
                table: "notifications",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__ratings__reviewe__18EBB532",
                table: "ratings",
                column: "reviewer_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__recruiter__user___59FA5E80",
                table: "recruiters",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__verificat__user___1F98B2C1",
                table: "verifications",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK__candidate__user___4CA06362",
                table: "candidates");

            migrationBuilder.DropForeignKey(
                name: "FK__companies__recru__52593CB8",
                table: "companies");

            migrationBuilder.DropForeignKey(
                name: "FK__notificat__user___2645B050",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK__ratings__reviewe__18EBB532",
                table: "ratings");

            migrationBuilder.DropForeignKey(
                name: "FK__recruiter__user___59FA5E80",
                table: "recruiters");

            migrationBuilder.DropForeignKey(
                name: "FK__verificat__user___1F98B2C1",
                table: "verifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK__users__B9BE370FA57EB208",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "(newid())");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "(newid())");

            migrationBuilder.AddPrimaryKey(
                name: "PK__users__B9BE370FA57EB208",
                table: "AspNetUsers",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__candidate__user___4CA06362",
                table: "candidates",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__companies__recru__52593CB8",
                table: "companies",
                column: "recruiter_id",
                principalTable: "AspNetUsers",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__notificat__user___2645B050",
                table: "notifications",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__ratings__reviewe__18EBB532",
                table: "ratings",
                column: "reviewer_id",
                principalTable: "AspNetUsers",
                principalColumn: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK__recruiter__user___59FA5E80",
                table: "recruiters",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__verificat__user___1F98B2C1",
                table: "verifications",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
