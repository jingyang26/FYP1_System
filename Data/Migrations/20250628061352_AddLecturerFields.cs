using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP1System.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLecturerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LecturerId",
                table: "Students",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanSupervise",
                table: "Lecturers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Specialization",
                table: "Lecturers",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_LecturerId",
                table: "Students",
                column: "LecturerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Lecturers_LecturerId",
                table: "Students",
                column: "LecturerId",
                principalTable: "Lecturers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Lecturers_LecturerId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_LecturerId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LecturerId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CanSupervise",
                table: "Lecturers");

            migrationBuilder.DropColumn(
                name: "Specialization",
                table: "Lecturers");
        }
    }
}
