using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedCascadeDeleteForEventTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_events_tags_events_id",
                table: "events_tags");

            migrationBuilder.DropForeignKey(
                name: "fk_events_tags_tags_id",
                table: "events_tags");

            migrationBuilder.AddForeignKey(
                name: "fk_events_tags_events_event_id",
                table: "events_tags",
                column: "event_id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_events_tags_tags_tag_id",
                table: "events_tags",
                column: "tag_id",
                principalTable: "tags",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_events_tags_events_event_id",
                table: "events_tags");

            migrationBuilder.DropForeignKey(
                name: "fk_events_tags_tags_tag_id",
                table: "events_tags");

            migrationBuilder.AddForeignKey(
                name: "fk_events_tags_events_id",
                table: "events_tags",
                column: "event_id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_events_tags_tags_id",
                table: "events_tags",
                column: "tag_id",
                principalTable: "tags",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
