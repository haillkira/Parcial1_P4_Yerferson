using Dapper;
using Microsoft.Data.Sqlite;
using Parcial1_P4_Yerferson.Models;

namespace Parcial1_P4_Yerferson.Services;

public class NumbersService(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Falta la conexion DefaultConnection.");

    private SqliteConnection CreateConnection() => new(_connectionString);

    public async Task InitializeAsync()
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS Numeros (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Fecha TEXT NOT NULL,
                Numero REAL NOT NULL,
                Resultado REAL NOT NULL
            );
            """;

        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql);
    }

    public async Task<int> SaveAsync(NumberRecord number)
    {
        const string sql = """
            INSERT INTO Numeros (Fecha, Numero, Resultado)
            VALUES (@Fecha, @Numero, @Resultado);

            SELECT last_insert_rowid();
            """;

        using var connection = CreateConnection();
        return await connection.ExecuteScalarAsync<int>(sql, number);
    }

    public async Task<bool> UpdateAsync(NumberRecord number)
    {
        const string sql = """
            UPDATE Numeros
            SET Fecha = @Fecha,
                Numero = @Numero,
                Resultado = @Resultado
            WHERE Id = @Id;
            """;

        using var connection = CreateConnection();
        int affectedRows = await connection.ExecuteAsync(sql, number);
        return affectedRows > 0;
    }

    public async Task<NumberRecord?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT Id, Fecha, Numero, Resultado
            FROM Numeros
            WHERE Id = @Id;
            """;

        using var connection = CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<NumberRow>(
            sql, new { Id = id });

        return row is null ? null : ToRecord(row);
    }

    public async Task<IEnumerable<NumberRecord>> GetListAsync()
    {
        const string sql = """
            SELECT Id, Fecha, Numero, Resultado
            FROM Numeros
            ORDER BY Id DESC;
            """;

        using var connection = CreateConnection();
        var rows = await connection.QueryAsync<NumberRow>(sql);
        return rows.Select(ToRecord).ToList();
    }

    private static NumberRecord ToRecord(NumberRow row) =>
        new(checked((int)row.Id), row.Fecha, row.Numero, row.Resultado);

    private sealed class NumberRow
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public double Numero { get; set; }
        public double Resultado { get; set; }
    }
}