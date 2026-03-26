using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiparisTakipSistemi.Migrations
{
    /// <inheritdoc />
    public partial class AddKullaniciIDToKargolar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KullaniciID",
                table: "Kargolar",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Kargolar_KargoFirmaID",
                table: "Kargolar",
                column: "KargoFirmaID");

            migrationBuilder.CreateIndex(
                name: "IX_Kargolar_KullaniciID",
                table: "Kargolar",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_SiparisDetaylari_SiparisID",
                table: "SiparisDetaylari",
                column: "SiparisID");

            migrationBuilder.CreateIndex(
                name: "IX_Siparisler_KargoID",
                table: "Siparisler",
                column: "KargoID");

            migrationBuilder.CreateIndex(
                name: "IX_Siparisler_KullaniciID",
                table: "Siparisler",
                column: "KullaniciID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Kargolar_KargoFirmaID",
                table: "Kargolar");

            migrationBuilder.DropIndex(
                name: "IX_Kargolar_KullaniciID",
                table: "Kargolar");

            migrationBuilder.DropIndex(
                name: "IX_SiparisDetaylari_SiparisID",
                table: "SiparisDetaylari");

            migrationBuilder.DropIndex(
                name: "IX_Siparisler_KargoID",
                table: "Siparisler");

            migrationBuilder.DropIndex(
                name: "IX_Siparisler_KullaniciID",
                table: "Siparisler");

            migrationBuilder.DropColumn(
                name: "KullaniciID",
                table: "Kargolar");
        }
    }
}
   
