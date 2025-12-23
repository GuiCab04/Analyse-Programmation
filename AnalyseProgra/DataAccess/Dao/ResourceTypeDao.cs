using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.DataAccess;

namespace AnalyseProgra.DataAccess.Dao
{
    public class ResourceTypeDao : IResourceTypeDao
    {
        public ResourceType? GetByName(string name)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT name FROM resource_types WHERE name = $name;";
            cmd.Parameters.AddWithValue("$name", name);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public IEnumerable<ResourceType> GetAll()
        {
            var list = new List<ResourceType>();

            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT name FROM resource_types;";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(Map(reader));

            return list;
        }

        public ResourceType Create(ResourceType type)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO resource_types (name)
                VALUES ($name);
            ";

            cmd.Parameters.AddWithValue("$name", type.Name);

            cmd.ExecuteNonQuery();
            return type;
        }

        public void Update(string name, ResourceType updated)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE resource_types
                SET name = $newName
                WHERE name = $name;
            ";

            cmd.Parameters.AddWithValue("$newName", updated.Name);
            cmd.Parameters.AddWithValue("$name", name);

            cmd.ExecuteNonQuery();
        }

        public void Delete(string name)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM resource_types WHERE name = $name;";
            cmd.Parameters.AddWithValue("$name", name);

            cmd.ExecuteNonQuery();
        }

        private static ResourceType Map(SqliteDataReader r)
        {
            return new ResourceType
            {
                Name = r.GetString(0)
            };
        }
    }
}
