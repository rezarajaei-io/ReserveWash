using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReserveWash.Migrations
{
    public partial class RevertUserIdToStrMIG : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_AspNetUsers_UserId1",
                table: "Feedback");

            migrationBuilder.DropIndex(
                name: "IX_Feedback_UserId1",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Feedback");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Feedback",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_UserId",
                table: "Feedback",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_AspNetUsers_UserId",
                table: "Feedback",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_AspNetUsers_UserId",
                table: "Feedback");

            migrationBuilder.DropIndex(
                name: "IX_Feedback_UserId",
                table: "Feedback");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Feedback",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Feedback",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_UserId1",
                table: "Feedback",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_AspNetUsers_UserId1",
                table: "Feedback",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
