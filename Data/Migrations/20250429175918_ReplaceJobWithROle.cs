using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceJobWithROle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Skapa RoleId som NULLABLE
            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "MemberProfile",
                type: "nvarchar(450)",
                nullable: true);

            // 2) Tilldela alla befintliga profiler User-rollens Id
            migrationBuilder.Sql(@"
        UPDATE mp
        SET mp.RoleId = r.Id
        FROM MemberProfile mp
        JOIN AspNetRoles r ON r.Name = 'User'
    ");

            // 3) Gör kolumnen NOT NULL
            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "MemberProfile",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            // 4) Skapa index och FK
            migrationBuilder.CreateIndex(
                name: "IX_MemberProfile_RoleId",
                table: "MemberProfile",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberProfile_AspNetRoles_RoleId",
                table: "MemberProfile",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // 5) Ta bort den gamla JobTitle-kolumnen
            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "MemberProfile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberProfile_AspNetRoles_RoleId",
                table: "MemberProfile");

            migrationBuilder.DropIndex(
                name: "IX_MemberProfile_RoleId",
                table: "MemberProfile");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "MemberProfile");

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "MemberProfile",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
