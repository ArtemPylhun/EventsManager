using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeletionAddedOnAttendances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attendances_events_id",
                table: "attendances");

            migrationBuilder.DropForeignKey(
                name: "fk_attendances_users_id",
                table: "attendances");

            migrationBuilder.AddForeignKey(
                name: "fk_attendances_events_id",
                table: "attendances",
                column: "event_id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_attendances_users_id",
                table: "attendances",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_attendances_events_id",
                table: "attendances");

            migrationBuilder.DropForeignKey(
                name: "fk_attendances_users_id",
                table: "attendances");

            migrationBuilder.AddForeignKey(
                name: "fk_attendances_events_id",
                table: "attendances",
                column: "event_id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_attendances_users_id",
                table: "attendances",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
