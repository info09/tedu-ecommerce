using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeduEcommerce.Migrations
{
    public partial class AddSellPriceToProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SellPrice",
                table: "AppProducts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SellPrice",
                table: "AppProducts");
        }
    }
}
