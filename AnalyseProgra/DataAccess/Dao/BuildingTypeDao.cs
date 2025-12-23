using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.DataAccess;

namespace AnalyseProgra.DataAccess.Dao
{
    public class BuildingTypeDao : IBuildingTypeDao
    {
        public BuildingType? GetByName(string name)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT name, description FROM building_types WHERE name = $name;";
            cmd.Parameters.AddWithValue("$name", name);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public IEnumerable<BuildingType> GetAll()
        {
            var list = new List<BuildingType>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT name, description FROM building_types;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(Map(reader));
            }

            return list;
        }

        public BuildingType Create(BuildingType type)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO building_types (name, description)
                VALUES ($name, $desc);
            ";

            cmd.Parameters.AddWithValue("$name", type.Name);
            cmd.Parameters.AddWithValue("$desc", (object?)type.Description ?? DBNull.Value);

            cmd.ExecuteNonQuery();
            return type;
        }

        public void Update(string name, BuildingType updated)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE building_types
                SET name = $newName,
                    description = $desc
                WHERE name = $name;
            ";

            cmd.Parameters.AddWithValue("$newName", updated.Name);
            cmd.Parameters.AddWithValue("$desc", (object?)updated.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$name", name);

            cmd.ExecuteNonQuery();
        }

        public void Delete(string name)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM building_types WHERE name = $name;";
            cmd.Parameters.AddWithValue("$name", name);

            cmd.ExecuteNonQuery();
        }

        private static BuildingType Map(SqliteDataReader r)
        {
            return new BuildingType
            {
                Name = r.GetString(0),
                Description = r.IsDBNull(1) ? null : r.GetString(1)
            };
        }
    }
}
