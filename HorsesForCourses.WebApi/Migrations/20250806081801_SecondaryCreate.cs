using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HorsesForCourses.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class SecondaryCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Coaches_CoachId1",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CoachId1",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CoachId1",
                table: "Courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoachId1",
                table: "Courses",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CoachId1",
                table: "Courses",
                column: "CoachId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Coaches_CoachId1",
                table: "Courses",
                column: "CoachId1",
                principalTable: "Coaches",
                principalColumn: "Id");
        }
    }
}
