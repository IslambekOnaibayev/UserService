using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class StatusAsString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Users"
                ALTER COLUMN "Status" TYPE character varying(20)
                USING CASE "Status"::integer
                    WHEN 1 THEN 'Unverified'
                    WHEN 2 THEN 'Active'
                    WHEN 3 THEN 'Blocked'
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE "Users"
                ALTER COLUMN "Status" TYPE integer
                USING CASE "Status"
                    WHEN 'Unverified' THEN 1
                    WHEN 'Active' THEN 2
                    WHEN 'Blocked' THEN 3
                END;
                """);
        }
    }
}
