using Microsoft.EntityFrameworkCore.Migrations;

namespace ReservationSystem.Migrations
{
    public partial class m03 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SCTimeslots_Sittings_SittingId",
                table: "SCTimeslots");

            migrationBuilder.DropIndex(
                name: "IX_SCTimeslots_SittingId",
                table: "SCTimeslots");

            migrationBuilder.DropColumn(
                name: "SittingId",
                table: "SCTimeslots");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SittingId",
                table: "SCTimeslots",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SCTimeslots_SittingId",
                table: "SCTimeslots",
                column: "SittingId");

            migrationBuilder.AddForeignKey(
                name: "FK_SCTimeslots_Sittings_SittingId",
                table: "SCTimeslots",
                column: "SittingId",
                principalTable: "Sittings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
