using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP1System.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedModelsForSupervisorEvaluator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StaffId",
                table: "Lecturers",
                type: "TEXT",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedAt",
                table: "EvaluatorAssignments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "EvaluationId",
                table: "EvaluatorAssignments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EvaluatorAssignments_EvaluationId",
                table: "EvaluatorAssignments",
                column: "EvaluationId");

            migrationBuilder.AddForeignKey(
                name: "FK_EvaluatorAssignments_ProposalEvaluations_EvaluationId",
                table: "EvaluatorAssignments",
                column: "EvaluationId",
                principalTable: "ProposalEvaluations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EvaluatorAssignments_ProposalEvaluations_EvaluationId",
                table: "EvaluatorAssignments");

            migrationBuilder.DropIndex(
                name: "IX_EvaluatorAssignments_EvaluationId",
                table: "EvaluatorAssignments");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Lecturers");

            migrationBuilder.DropColumn(
                name: "AssignedAt",
                table: "EvaluatorAssignments");

            migrationBuilder.DropColumn(
                name: "EvaluationId",
                table: "EvaluatorAssignments");
        }
    }
}
