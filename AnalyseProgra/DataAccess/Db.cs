using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace AnalyseProgra.DataAccess
{
    public static class Db
    {
        public static SqliteConnection GetConnection()
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "game.db");

            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = dbPath,
                ForeignKeys = true
            };

            return new SqliteConnection(builder.ToString());
        }
    }
}
