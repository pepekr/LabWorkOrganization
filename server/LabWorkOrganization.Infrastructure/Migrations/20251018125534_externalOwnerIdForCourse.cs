using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabWorkOrganization.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class externalOwnerIdForCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerExternalId",
                table: "Courses",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerExternalId",
                table: "Courses");
        }
    }
}
