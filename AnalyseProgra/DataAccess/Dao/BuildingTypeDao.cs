using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.DataAccess;

namespace AnalyseProgra.DataAccess.Dao
{
    public class BuildingTypeDao : IBuildingTypeDao
    {
        public BuildingType? GetById(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, name, description FROM building_types WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public BuildingType? GetByName(string name)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, name, description FROM building_types WHERE name = $name;";
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
            cmd.CommandText = @"SELECT id, name, description FROM building_types;";

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
                SELECT last_insert_rowid();
            ";

            cmd.Parameters.AddWithValue("$name", type.Name);
            cmd.Parameters.AddWithValue("$desc", (object?)type.Description ?? DBNull.Value);

            var id = (long)cmd.ExecuteScalar();
            type.Id = (int)id;

            return type;
        }

        public void Update(BuildingType type)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE building_types
                SET name = $name,
                    description = $desc
                WHERE id = $id;
            ";

            cmd.Parameters.AddWithValue("$name", type.Name);
            cmd.Parameters.AddWithValue("$desc", (object?)type.Description ?? DBNull.Value);
            cmd.Parameters.AddWithValue("$id", type.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM building_types WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            cmd.ExecuteNonQuery();
        }

        private static BuildingType Map(SqliteDataReader r)
        {
            return new BuildingType
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                Description = r.IsDBNull(2) ? null : r.GetString(2)
            };
        }
    }
}
