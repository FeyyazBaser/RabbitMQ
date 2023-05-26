using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RabbitMQ.Web.Excel.Migrations
{
    /// <inheritdoc />
    public partial class updateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_userFiles",
                table: "userFiles");

            migrationBuilder.RenameTable(
                name: "userFiles",
                newName: "UserFiles");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "UserFiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserFiles",
                table: "UserFiles",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserFiles",
                table: "UserFiles");

            migrationBuilder.RenameTable(
                name: "UserFiles",
                newName: "userFiles");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "userFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_userFiles",
                table: "userFiles",
                column: "Id");
        }
    }
}
