using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabWorkOrganization.Infrastructure.Migrations
{
    public partial class CourseNameChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Only drop FK if it exists - safer to comment out problematic drops
            // migrationBuilder.DropForeignKey(
            //     name: "FK_SubGroups_Course_CourseId",
            //     table: "SubGroups");

            // migrationBuilder.DropForeignKey(
            //     name: "FK_Tasks_Course_CourseId",
            //     table: "Tasks");

            // migrationBuilder.DropForeignKey(
            //     name: "FK_Users_Course_CourseId",
            //     table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Course",
                table: "Course");

            migrationBuilder.RenameTable(
                name: "Course",
                newName: "Courses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                table: "Courses",
                column: "Id");

            // Re-add FKs pointing to new table name
            migrationBuilder.AddForeignKey(
                name: "FK_SubGroups_Courses_CourseId",
                table: "SubGroups",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Courses_CourseId",
                table: "Tasks",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Courses_CourseId",
                table: "Users",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove FKs pointing to renamed table
            migrationBuilder.DropForeignKey(
                name: "FK_SubGroups_Courses_CourseId",
                table: "SubGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Courses_CourseId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Courses_CourseId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                table: "Courses");

            migrationBuilder.RenameTable(
                name: "Courses",
                newName: "Course");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Course",
                table: "Course",
                column: "Id");

            // Re-add FKs pointing to old table name
            // Only do this if the original FKs existed in your DB
            // Otherwise comment out
            // migrationBuilder.AddForeignKey(
            //     name: "FK_SubGroups_Course_CourseId",
            //     table: "SubGroups",
            //     column: "CourseId",
            //     principalTable: "Course",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Tasks_Course_CourseId",
            //     table: "Tasks",
            //     column: "CourseId",
            //     principalTable: "Course",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);

            // migrationBuilder.AddForeignKey(
            //     name: "FK_Users_Course_CourseId",
            //     table: "Users",
            //     column: "CourseId",
            //     principalTable: "Course",
            //     principalColumn: "Id");
        }
    }
}
