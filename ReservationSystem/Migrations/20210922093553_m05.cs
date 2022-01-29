using Microsoft.EntityFrameworkCore.Migrations;

namespace ReservationSystem.Migrations
{
    public partial class m05 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Reservations",
                newName: "ExpectedStartTime");

            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Reservations",
                newName: "ExpectedEndTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpectedStartTime",
                table: "Reservations",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "ExpectedEndTime",
                table: "Reservations",
                newName: "Duration");
        }
    }
}
