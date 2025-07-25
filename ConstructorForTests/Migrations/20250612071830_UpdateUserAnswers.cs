﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructorForTests.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NeedVerification",
                table: "UserAnswers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedVerification",
                table: "UserAnswers");
        }
    }
}
