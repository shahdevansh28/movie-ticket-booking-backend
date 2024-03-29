using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace movie_ticket_booking.Migrations
{
    /// <inheritdoc />
    public partial class postgresdbV14 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "ShowTimes",
                newName: "row");

            migrationBuilder.AddColumn<int>(
                name: "column",
                table: "ShowTimes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "column",
                table: "ShowTimes");

            migrationBuilder.RenameColumn(
                name: "row",
                table: "ShowTimes",
                newName: "Capacity");
        }
    }
}
