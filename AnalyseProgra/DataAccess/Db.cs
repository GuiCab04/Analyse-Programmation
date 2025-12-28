using Microsoft.Data.Sqlite;
using System;
using System.IO;

public static class Db
{
    private static readonly string DbPath = Path.Combine(
        Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName,
        "game.db"
    );

    public static SqliteConnection GetConnection()
    {
        return new SqliteConnection($"Data Source={DbPath};Foreign Keys=True;");
    }
}


// cree le fichier game.db
// cmd /c ".\sqlite3.exe game.db < ap.sql"

