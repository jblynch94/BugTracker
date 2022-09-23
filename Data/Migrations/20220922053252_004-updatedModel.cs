using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugTracker.Data.Migrations
{
    public partial class _004updatedModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileDate",
                table: "TicketAttachment",
                newName: "FileData");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "TicketAttachment",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "TicketAttachment");

            migrationBuilder.RenameColumn(
                name: "FileData",
                table: "TicketAttachment",
                newName: "FileDate");
        }
    }
}
