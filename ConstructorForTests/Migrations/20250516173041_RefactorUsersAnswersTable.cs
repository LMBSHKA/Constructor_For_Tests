using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructorForTests.Migrations
{
    /// <inheritdoc />
    public partial class RefactorUsersAnswersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MultipleChoice",
                table: "MultipleChoice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchingPair",
                table: "MatchingPair");

            migrationBuilder.RenameTable(
                name: "MultipleChoice",
                newName: "MultipleChoices");

            migrationBuilder.RenameTable(
                name: "MatchingPair",
                newName: "MatchingPairs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MultipleChoices",
                table: "MultipleChoices",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchingPairs",
                table: "MatchingPairs",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserMatchingPairs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PairId = table.Column<Guid>(type: "uuid", nullable: false),
                    PairKey = table.Column<string>(type: "text", nullable: false),
                    PairValue = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMatchingPairs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserMultipleChoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MultipleAnswerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Answer = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMultipleChoices", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMatchingPairs");

            migrationBuilder.DropTable(
                name: "UserMultipleChoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MultipleChoices",
                table: "MultipleChoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchingPairs",
                table: "MatchingPairs");

            migrationBuilder.RenameTable(
                name: "MultipleChoices",
                newName: "MultipleChoice");

            migrationBuilder.RenameTable(
                name: "MatchingPairs",
                newName: "MatchingPair");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MultipleChoice",
                table: "MultipleChoice",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchingPair",
                table: "MatchingPair",
                column: "Id");
        }
    }
}
