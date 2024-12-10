using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant_Manager.Migrations
{
    /// <inheritdoc />
    public partial class remove_useless_entry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                table: "aspnet_users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "aspnet_users",
                type: "text",
                nullable: true);
        }
    }
}
