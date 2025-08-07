using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QarzDaftar.Server.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDebtModel_RemoveUserId_AddRemainingAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Customers_CustomerId",
                table: "Debts");

            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Users_UserId",
                table: "Debts");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Debts",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<decimal>(
                name: "RemainingAmount",
                table: "Debts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Customers_CustomerId",
                table: "Debts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Users_UserId",
                table: "Debts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Customers_CustomerId",
                table: "Debts");

            migrationBuilder.DropForeignKey(
                name: "FK_Debts_Users_UserId",
                table: "Debts");

            migrationBuilder.DropColumn(
                name: "RemainingAmount",
                table: "Debts");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Debts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Customers_CustomerId",
                table: "Debts",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Debts_Users_UserId",
                table: "Debts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
