using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookSwap1.Migrations
{
    public partial class eيةةزز : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Chats");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Chats",
                newName: "SentDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SentDate",
                table: "Chats",
                newName: "SentAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Chats",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
