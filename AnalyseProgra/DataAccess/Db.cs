using Microsoft.Data.Sqlite;
using System;

public static class Db
{
    public static SqliteConnection GetConnection()
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "game.db");

        return new SqliteConnection(
            $"Data Source={dbPath};Foreign Keys=True;"
        );
    }
}

// cree le fichier game.db
// cmd /c ".\sqlite3.exe game.db < ap.sql"

