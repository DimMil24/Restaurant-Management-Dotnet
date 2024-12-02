using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Restaurant_Manager.Migrations
{
    /// <inheritdoc />
    public partial class remove_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_customer_order_customer_table_customer_table_id",
                table: "customer_order");

            migrationBuilder.DropTable(
                name: "customer_table");

            migrationBuilder.DropIndex(
                name: "ix_customer_order_customer_table_id",
                table: "customer_order");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "customer_table",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    restaurant_id = table.Column<long>(type: "bigint", nullable: false),
                    available = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_customer_table", x => x.id);
                    table.ForeignKey(
                        name: "fk_customer_table_restaurant_restaurant_id",
                        column: x => x.restaurant_id,
                        principalTable: "restaurant",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_customer_order_customer_table_id",
                table: "customer_order",
                column: "customer_table_id");

            migrationBuilder.CreateIndex(
                name: "ix_customer_table_restaurant_id",
                table: "customer_table",
                column: "restaurant_id");

            migrationBuilder.AddForeignKey(
                name: "fk_customer_order_customer_table_customer_table_id",
                table: "customer_order",
                column: "customer_table_id",
                principalTable: "customer_table",
                principalColumn: "id");
        }
    }
}
