using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructorForTests.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailureMessage",
                table: "Tests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MessageAboutPassing",
                table: "Tests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimerInSeconds",
                table: "Tests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Tests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AnswerOptions",
                table: "Questions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PairKey",
                table: "Questions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PairValue",
                table: "Questions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailureMessage",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "MessageAboutPassing",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "TimerInSeconds",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "AnswerOptions",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "PairKey",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "PairValue",
                table: "Questions");
        }
    }
}
