using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConstructorForTests.Migrations
{
    /// <inheritdoc />
    public partial class AddedDeletionTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Tests",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Tests");
        }
    }
}
