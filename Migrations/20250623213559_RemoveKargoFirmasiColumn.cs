using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiparisTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveKargoFirmasiColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kargolar_Kullanicilar_KullaniciID",
                table: "Kargolar");

            migrationBuilder.DropIndex(
                name: "IX_Kargolar_KullaniciID",
                table: "Kargolar");

            migrationBuilder.DropColumn(
                name: "KullaniciID",
                table: "Kargolar");

            migrationBuilder.DropColumn(
                name: "OlusturmaTarihi",
                table: "Kargolar");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KullaniciID",
                table: "Kargolar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "OlusturmaTarihi",
                table: "Kargolar",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Kargolar_KullaniciID",
                table: "Kargolar",
                column: "KullaniciID");

            migrationBuilder.AddForeignKey(
                name: "FK_Kargolar_Kullanicilar_KullaniciID",
                table: "Kargolar",
                column: "KullaniciID",
                principalTable: "Kullanicilar",
                principalColumn: "KullaniciID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
