using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Amccloy.MusicBot.Net.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "trivia_questions");

            migrationBuilder.CreateTable(
                name: "multi_choice_trivia_questions",
                schema: "trivia_questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category = table.Column<int>(type: "integer", nullable: false),
                    question = table.Column<string>(type: "text", nullable: true),
                    answer = table.Column<string>(type: "text", nullable: true),
                    false_answers = table.Column<string[]>(type: "jsonb", nullable: true),
                    source = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_multi_choice_trivia_questions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "standard_trivia_questions",
                schema: "trivia_questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    category = table.Column<int>(type: "integer", nullable: false),
                    question = table.Column<string>(type: "text", nullable: true),
                    answer = table.Column<string>(type: "text", nullable: true),
                    source = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_standard_trivia_questions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_multi_choice_trivia_questions_category",
                schema: "trivia_questions",
                table: "multi_choice_trivia_questions",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_multi_choice_trivia_questions_source",
                schema: "trivia_questions",
                table: "multi_choice_trivia_questions",
                column: "source");

            migrationBuilder.CreateIndex(
                name: "ix_standard_trivia_questions_category",
                schema: "trivia_questions",
                table: "standard_trivia_questions",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "ix_standard_trivia_questions_source",
                schema: "trivia_questions",
                table: "standard_trivia_questions",
                column: "source");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "multi_choice_trivia_questions",
                schema: "trivia_questions");

            migrationBuilder.DropTable(
                name: "standard_trivia_questions",
                schema: "trivia_questions");
        }
    }
}
