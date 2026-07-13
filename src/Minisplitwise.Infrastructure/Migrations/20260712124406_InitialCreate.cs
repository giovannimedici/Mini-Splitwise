using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniSplitwise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "groups",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "expenses",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: false),
                    amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    group_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    paid_by_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    for_everyone = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_expenses", x => x.id);
                    table.ForeignKey(
                        name: "fk_expenses_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    birth_date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    expense_id = table.Column<Guid>(type: "TEXT", nullable: true),
                    group_id = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_members_expenses_expense_id",
                        column: x => x.expense_id,
                        principalTable: "expenses",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_members_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_expenses_group_id",
                table: "expenses",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_expenses_paid_by_id",
                table: "expenses",
                column: "paid_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_members_expense_id",
                table: "members",
                column: "expense_id");

            migrationBuilder.CreateIndex(
                name: "ix_members_group_id",
                table: "members",
                column: "group_id");

            migrationBuilder.AddForeignKey(
                name: "fk_expenses_members_paid_by_id",
                table: "expenses",
                column: "paid_by_id",
                principalTable: "members",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_expenses_groups_group_id",
                table: "expenses");

            migrationBuilder.DropForeignKey(
                name: "fk_members_groups_group_id",
                table: "members");

            migrationBuilder.DropForeignKey(
                name: "fk_expenses_members_paid_by_id",
                table: "expenses");

            migrationBuilder.DropTable(
                name: "groups");

            migrationBuilder.DropTable(
                name: "members");

            migrationBuilder.DropTable(
                name: "expenses");
        }
    }
}
