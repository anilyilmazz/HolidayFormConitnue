using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace IzinFormu.Data.Migrations
{
    public partial class migr4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Departman_ManagerId",
                table: "Departman");

            migrationBuilder.CreateIndex(
                name: "IX_Departman_ManagerId",
                table: "Departman",
                column: "ManagerId",
                unique: true,
                filter: "[ManagerId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Departman_ManagerId",
                table: "Departman");

            migrationBuilder.CreateIndex(
                name: "IX_Departman_ManagerId",
                table: "Departman",
                column: "ManagerId");
        }
    }
}
