using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiparisTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class AddToplamToSiparisDetaylari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Toplam",
                table: "SiparisDetaylari",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Toplam",
                table: "SiparisDetaylari");
        }
    }
}
