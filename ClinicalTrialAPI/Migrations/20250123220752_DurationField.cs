using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicalTrialAPI.Migrations
{
    /// <inheritdoc />
    public partial class DurationField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.AddColumn<int>(
                name: "TrialDuration",
                table: "ClinicalTrialMetadata",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.DropColumn(
                name: "TrialDuration",
                table: "ClinicalTrialMetadata");
        }
    }
}
