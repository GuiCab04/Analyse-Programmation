using AnalyseProgra.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using AnalyseProgra.DataAccess.Interface;
using AnalyseProgra.DataAccess;

namespace AnalyseProgra.DataAccess.Dao
{
    public class ResourceTypeDao : IResourceTypeDao
    {
        public ResourceType? GetById(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, name FROM resource_types WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? Map(reader) : null;
        }

        public ResourceType? GetByName(string name)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT id, name FROM resource_types WHERE name = $name;";
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
            cmd.CommandText = "SELECT id, name FROM resource_types;";

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
                SELECT last_insert_rowid();
            ";

            cmd.Parameters.AddWithValue("$name", type.Name);

            var newId = (long)cmd.ExecuteScalar();
            type.Id = (int)newId;

            return type;
        }

        public void Update(ResourceType type)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                UPDATE resource_types
                SET name = $name
                WHERE id = $id;
            ";

            cmd.Parameters.AddWithValue("$name", type.Name);
            cmd.Parameters.AddWithValue("$id", type.Id);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = Db.GetConnection();
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM resource_types WHERE id = $id;";
            cmd.Parameters.AddWithValue("$id", id);

            cmd.ExecuteNonQuery();
        }

        private static ResourceType Map(SqliteDataReader r)
        {
            return new ResourceType
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1)
            };
        }
    }
}
