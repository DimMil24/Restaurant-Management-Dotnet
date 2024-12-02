using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurant_Manager.Migrations
{
    /// <inheritdoc />
    public partial class Add_description_product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "customer_table_id",
                table: "customer_order");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "product",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "product");

            migrationBuilder.AddColumn<long>(
                name: "customer_table_id",
                table: "customer_order",
                type: "bigint",
                nullable: true);
        }
    }
}
