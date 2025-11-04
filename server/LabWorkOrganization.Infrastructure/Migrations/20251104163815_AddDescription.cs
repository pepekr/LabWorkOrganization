using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabWorkOrganization.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TaskId",
                table: "QueuePlaces",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_QueuePlaces_TaskId",
                table: "QueuePlaces",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_QueuePlaces_Tasks_TaskId",
                table: "QueuePlaces",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueuePlaces_Tasks_TaskId",
                table: "QueuePlaces");

            migrationBuilder.DropIndex(
                name: "IX_QueuePlaces_TaskId",
                table: "QueuePlaces");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "QueuePlaces");
        }
    }
}
