using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using AppForSEII2526.API.Data;

namespace AppForSEII2526.UT
{
    public class AppForSEII25264SqliteUT
    {
        protected readonly DbConnection _connection;
        protected readonly ApplicationDbContext _context;
        protected readonly DbContextOptions<ApplicationDbContext> _contextOptions;

        // Método para crear un nuevo contexto limpio cuando lo necesitemos
        protected ApplicationDbContext CreateContext() => new(_contextOptions);

        public void Dispose() => _connection.Dispose();

        public AppForSEII25264SqliteUT()
        {
            // 1. Creamos la conexión a SQLite en memoria (Filename=:memory:)
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            // 2. Configuramos las opciones para que EF Core use esa conexión de SQLite
            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            // 3. Inicializamos el contexto y creamos las tablas
            _context = new ApplicationDbContext(_contextOptions);

            if (_context.Database.EnsureCreated())
            {
                // Creamos una vista de ejemplo por si el profesor la pide (opcional)
                using var viewCommand = _context.Database.GetDbConnection().CreateCommand();
                viewCommand.CommandText = @"
                    CREATE VIEW AllDevices AS
                    SELECT Name
                    FROM Devices;";
                viewCommand.ExecuteNonQuery();
            }
        }
    }
}