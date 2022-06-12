using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Amccloy.MusicBot.Net.Migrations
{
    public partial class MusicTrivia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "music_trivia_questions",
                schema: "trivia_questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    source = table.Column<int>(type: "integer", nullable: false),
                    playlist = table.Column<string>(type: "text", nullable: true),
                    song_name = table.Column<string>(type: "text", nullable: true),
                    song_artist = table.Column<string>(type: "text", nullable: true),
                    song_album = table.Column<string>(type: "text", nullable: true),
                    song_year = table.Column<string>(type: "text", nullable: true),
                    song_original_artist = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_music_trivia_questions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_music_trivia_questions_playlist",
                schema: "trivia_questions",
                table: "music_trivia_questions",
                column: "playlist");

            migrationBuilder.CreateIndex(
                name: "ix_music_trivia_questions_source",
                schema: "trivia_questions",
                table: "music_trivia_questions",
                column: "source");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "music_trivia_questions",
                schema: "trivia_questions");
        }
    }
}
