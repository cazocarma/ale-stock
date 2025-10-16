using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AleStock.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO ""Usuarios"" (""Nombre"", ""Email"", ""PasswordHash"", ""Rol"")
                VALUES ('Alejandro Neira', 'alejandro@alestock.local', 'admin123', 'Coordinador');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
