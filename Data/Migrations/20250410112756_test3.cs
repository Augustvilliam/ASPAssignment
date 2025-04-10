using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class test3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberEntity_AspNetUsers_MemberId",
                table: "MemberEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberEntity",
                table: "MemberEntity");

            migrationBuilder.RenameTable(
                name: "MemberEntity",
                newName: "MemberProfile");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberProfile",
                table: "MemberProfile",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberProfile_AspNetUsers_MemberId",
                table: "MemberProfile",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MemberProfile_AspNetUsers_MemberId",
                table: "MemberProfile");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberProfile",
                table: "MemberProfile");

            migrationBuilder.RenameTable(
                name: "MemberProfile",
                newName: "MemberEntity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberEntity",
                table: "MemberEntity",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_MemberEntity_AspNetUsers_MemberId",
                table: "MemberEntity",
                column: "MemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
