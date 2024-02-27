using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BaseProject.DAO.Migrations
{
    /// <inheritdoc />
    public partial class AddProcLimparLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			var sql = @"
                CREATE OR ALTER PROCEDURE LimparLogs (@N INT)
                AS
                BEGIN
                    --Limpa os logs de N dias atrás
	                DELETE FROM Log WHERE TimeStamp <= DATEADD(DAY, -@N, CAST(GETDATE() AS DATE))
                END
            ";

			migrationBuilder.Sql(sql);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
