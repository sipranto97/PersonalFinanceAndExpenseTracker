using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceAndExpenseTracker.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "UserAccounts",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "USD");

            migrationBuilder.AddColumn<bool>(
                name: "NeedsCurrencySetup",
                table: "UserAccounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "UserAccounts");

            migrationBuilder.DropColumn(
                name: "NeedsCurrencySetup",
                table: "UserAccounts");
        }
    }
}
